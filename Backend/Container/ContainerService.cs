using Database.Migrations;
using Database.Models;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Container;

public interface IContainerService
{
    Task CreateImage(Analyzer analyzer, CancellationToken cancellationToken);
    Task DeleteImage(Guid analyzerId);
    Task<string> CreateContainer(Guid analyzerId, Guid entryId, CancellationToken cancellationToken);
    Task<(string LogInformation, string LogError)> GetLogs(string container, CancellationToken cancellationToken);
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

    public async Task CreateImage(Analyzer analyzer, CancellationToken cancellationToken)
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

        await _dockerClient.Images.BuildImageFromDockerfileAsync
        (
            new ImageBuildParameters
            {
                Tags = [$"analyzer-{analyzer.Id}"],
                Remove = true,
                ForceRemove = true,
            },
            tarStream,
            null,
            null,
            new Progress<JSONMessage>(message =>
            {
                if (message.Error is not null)
                {
                    _logger.LogError("CreateImage analyzer-{ID}:\n{MSG}", analyzer.Id, message.ErrorMessage.Trim());
                }
                else if (!string.IsNullOrWhiteSpace(message.Stream))
                {
                    _logger.LogInformation("CreateImage analyzer-{ID}:\n{MSG}", analyzer.Id, message.Stream.Trim());
                }
            }),
            cancellationToken
        );
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
                }
            },
            cancellationToken
        );

        return container.ID;
    }

    public async Task<(string LogInformation, string LogError)> GetLogs(string container, CancellationToken cancellationToken)
    {
        var logsStream = await _dockerClient.Containers.GetContainerLogsAsync(container, false, new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true,
            Follow = true
        }, cancellationToken);

        return await logsStream.ReadOutputToEndAsync(cancellationToken);
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
