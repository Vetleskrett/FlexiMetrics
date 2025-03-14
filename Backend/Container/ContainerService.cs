using System.Text.Json;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;

namespace Container;

public interface IContainerService
{
    Task Initialize();
    Task<Stream?> CopyFileFromContainer(string container, string path);
    Task CopyFileToContainer(string container, Stream stream, string fileName);
    Task<string> CreateContainer(string name, CancellationToken cancellationToken);
    Task<(string LogInformation, string LogError)> GetLogs(string container, CancellationToken cancellationToken);
    Task RemoveContainer(string container);
    Task StartContainer(string container, CancellationToken cancellationToken);
    Task WaitForContainerCompletion(string container, CancellationToken cancellationToken);
}

public class ContainerService : IContainerService
{
    private const string IMAGE = "python:3.13-slim";
    private const string WORKING_DIR = "/app";

    private readonly IDockerClient _dockerClient;
    private readonly ILogger<ContainerService> _logger;

    public ContainerService(IDockerClient dockerClient, ILogger<ContainerService> logger)
    {
        _dockerClient = dockerClient;
        _logger = logger;
    }

    public async Task Initialize()
    {
        await _dockerClient.Images.CreateImageAsync
        (
        new ImagesCreateParameters
        {
            FromImage = IMAGE
        },
            null,
            new Progress<JSONMessage>(message =>
            {
                if (message.Error is not null)
                {
                    _logger.LogError("Container.Initialize: {PROGRESS}", JsonSerializer.Serialize(message));
                }
                else
                {
                    _logger.LogInformation("Container.Initialize: {PROGRESS}", JsonSerializer.Serialize(message));
                }
            })
        );
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

    public async Task<string> CreateContainer(string name, CancellationToken cancellationToken)
    {
        var container = await _dockerClient.Containers.CreateContainerAsync
        (
            new CreateContainerParameters
            {
                Image = IMAGE,
                Name = name,
                WorkingDir = WORKING_DIR,
                Cmd = ["python", "script.py"],
                HostConfig = new HostConfig
                {
                    AutoRemove = false,
                }
            },
            cancellationToken
        );
        return container.ID;
    }

    public async Task CopyFileToContainer(string container, Stream stream, string fileName)
    {
        using var tarStream = TarArchive.Create(stream, fileName);
        await _dockerClient.Containers.ExtractArchiveToContainerAsync
        (
            container,
            new ContainerPathStatParameters
            {
                Path = WORKING_DIR
            },
            tarStream
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
}
