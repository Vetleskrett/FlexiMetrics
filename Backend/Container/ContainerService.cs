using Database.Models;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using System.Diagnostics;
using System.Text;
using System.Text.Unicode;
using System.Threading.Channels;

namespace Container;

public interface IContainerService
{
    Task CreateImage(Analyzer analyzer, ChannelWriter<AnalyzerLog> logsWritter, CancellationToken cancellationToken);
    Task DeleteImage(Guid analyzerId);
    Task<string> CreateContainer(Guid analyzerId, Guid entryId, CancellationToken cancellationToken);
    Task<(string stdout, string stderr)> GetLogStream(string container, CancellationToken cancellationToken);
    Task RemoveContainer(string container);
    Task StartContainer(string container, CancellationToken cancellationToken);
    Task WaitForContainerCompletion(string container, CancellationToken cancellationToken);
    Task CopyFileToContainer(string container, Stream stream, string fileName, CancellationToken cancellationToken);
    Task CopyFileToContainer(string container, string contents, string fileName, CancellationToken cancellationToken);
    Task<Stream?> CopyFileFromContainer(string container, string path);
}

public class ContainerService : IContainerService
{
    private const string DOCKERFILE_PATH = "../Container/Scripts/Dockerfile";
    private const string FLEXIMETRICS_PATH = "../Container/Scripts/fleximetrics.py";
    private const string WORKING_DIR = "/app";

    private readonly IDockerClient _dockerClient;
    private readonly ILogger<ContainerService> _logger;

    public ContainerService(IDockerClient dockerClient, ILogger<ContainerService> logger)
    {
        _dockerClient = dockerClient;
        _logger = logger;
    }

    public async Task CreateImage(Analyzer analyzer, ChannelWriter<AnalyzerLog> logsWritter, CancellationToken cancellationToken)
    {
        try
        {
            var dockerfile = await File.ReadAllTextAsync(DOCKERFILE_PATH, cancellationToken);
            var fleximetrics = await File.ReadAllTextAsync(FLEXIMETRICS_PATH, cancellationToken);

            var tarStream = TarArchive.CreateAll
            (
                new TextFile("Dockerfile", dockerfile),
                new TextFile("fleximetrics.py", fleximetrics),
                new TextFile("requirements.txt", analyzer.Requirements),
                new TextFile("packages-list.txt", analyzer.AptPackages)
            );

            var progress = new Progress<JSONMessage>(message =>
            {
                if (message.Error is not null)
                {
                    var log = new AnalyzerLog
                    {
                        Id = Guid.NewGuid(),
                        Timestamp = DateTime.UtcNow,
                        Type = AnalyzerLogType.Error,
                        Category = analyzer.Name,
                        Text = message.ErrorMessage.Trim(),
                        AnalyzerId = analyzer.Id
                    };

                    while (!logsWritter.TryWrite(log)) { }
                }
                else if (message.Stream is not null)
                {
                    var log = new AnalyzerLog
                    {
                        Id = Guid.NewGuid(),
                        Timestamp = DateTime.UtcNow,
                        Type = AnalyzerLogType.Information,
                        Category = analyzer.Name,
                        Text = message.Stream,
                        AnalyzerId = analyzer.Id
                    };

                    while (!logsWritter.TryWrite(log)) { }
                }
            });

            await _dockerClient.Images.BuildImageFromDockerfileAsync
            (
                new ImageBuildParameters
                {
                    Tags = [$"analyzer-{analyzer.Id}"],
                    Remove = true,
                    ForceRemove = true,
                    NoCache = true
                },
                tarStream,
                null,
                null,
                progress,
                cancellationToken
            );
        }
        finally
        {
            logsWritter.Complete();
        }
    }

    public async Task DeleteImage(Guid analyzerId)
    {
        try
        {
            await _dockerClient.Images.DeleteImageAsync($"analyzer-{analyzerId}", new ImageDeleteParameters
            {
                Force = true
            });
        }
        catch (DockerImageNotFoundException) { }
    }

    public async Task<string> CreateContainer(Guid analyzerId, Guid entryId, CancellationToken cancellationToken)
    {
        var container = await _dockerClient.Containers.CreateContainerAsync
        (
            new CreateContainerParameters
            {
                Image = $"analyzer-{analyzerId}",
                Name = $"analyzer-{analyzerId}-{entryId}",
                WorkingDir = WORKING_DIR,
                HostConfig = new HostConfig
                {
                    AutoRemove = false,
                },
            },
            cancellationToken
        );

        return container.ID;
    }

    public async Task<(string stdout, string stderr)> GetLogStream(string container, CancellationToken cancellationToken)
    {
        var logStream = await _dockerClient.Containers.GetContainerLogsAsync(container, false, new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true,
            Follow = true
        }, cancellationToken);

        return await logStream.ReadOutputToEndAsync(cancellationToken);
    }

    public async Task StartContainer(string container, CancellationToken cancellationToken)
    {
        await _dockerClient.Containers.StartContainerAsync(container, new ContainerStartParameters(), cancellationToken);
    }

    public async Task WaitForContainerCompletion(string container, CancellationToken cancellationToken)
    {
        await _dockerClient.Containers.WaitContainerAsync(container, cancellationToken);
    }

    public async Task RemoveContainer(string container)
    {
        try
        {
            await _dockerClient.Containers.RemoveContainerAsync
            (
                container,
                new ContainerRemoveParameters
                {
                    Force = true
                },
                CancellationToken.None
            );
        }
        catch (Exception) { }
    }

    public async Task CopyFileToContainer(string container, string contents, string fileName, CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(contents);
        using var stream = new MemoryStream(bytes);
        await CopyFileToContainer(container, stream, fileName, cancellationToken);
    }

    public async Task CopyFileToContainer(string container, Stream stream, string fileName, CancellationToken cancellationToken)
    {
        using var tarStream = TarArchive.Create(stream, fileName);
        await _dockerClient.Containers.ExtractArchiveToContainerAsync
        (
            container,
            new ContainerPathStatParameters
            {
                Path = WORKING_DIR
            },
            tarStream,
            cancellationToken
        );
    }

    public async Task<Stream?> CopyFileFromContainer(string container, string path)
    {
        try
        {
            var response = await _dockerClient.Containers.GetArchiveFromContainerAsync
            (
                container,
                new GetArchiveFromContainerParameters
                {
                    Path = $"{WORKING_DIR}/{path}"
                },
                false
            );
            return TarArchive.Extract(response.Stream);
        }
        catch
        {
            return null;
        }
    }
}
