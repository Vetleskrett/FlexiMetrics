using Database;
using FileStorage;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using Database.Models;

namespace Container;

public interface IContainerService
{
    Task Initialize();
    Task StartAnalyzer(Guid CourseId, Guid AssignmentId, Guid AnalyzerId);
    Task RunAnalyzer(RunAnalyzerRequest request, CancellationToken cancellationToken);
}

public class ContainerService : IContainerService
{
    private const string IMAGE = "python:3.13-slim";
    private const string FLEXIMETRICS_PATH = "../Container/fleximetrics.py";
    private const string WORKING_DIR = "/app";

    private readonly AppDbContext _dbContext;
    private readonly IFileStorage _fileStorage;
    private readonly Channel<RunAnalyzerRequest> _channel;
    private readonly IDockerClient _dockerClient;
    private readonly ILogger<ContainerService> _logger;

    public ContainerService(AppDbContext dbContext, IFileStorage fileStorage, Channel<RunAnalyzerRequest> channel, IDockerClient dockerClient, ILogger<ContainerService> logger)
    {
        _dbContext = dbContext;
        _fileStorage = fileStorage;
        _channel = channel;
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

        var containers = await _dockerClient.Containers.ListContainersAsync(new());
        foreach (var container in containers)
        {
            if (container.Names.Any(n => n.StartsWith("analyzer")))
            {
                await _dockerClient.Containers.RemoveContainerAsync(container.ID, new());
            }
        }
    }

    public async Task StartAnalyzer(Guid CourseId, Guid AssignmentId, Guid AnalyzerId)
    {
        await _channel.Writer.WriteAsync(new(CourseId, AssignmentId, AnalyzerId));
    }

    public async Task RunAnalyzer(RunAnalyzerRequest request, CancellationToken cancellationToken)
    {
        var deliveries = await _dbContext.Deliveries
            .Include(d => d.Student)
            .Include(d => d.Team)
            .ThenInclude(t => t!.Students)
            .Include(d => d.Fields!)
            .ThenInclude(f => f.AssignmentField)
            .Where(d => d.AssignmentId == request.AssignmentId)
            .ToListAsync();

        await Parallel.ForEachAsync(deliveries, cancellationToken, async (delivery, cancellationToken) =>
        {
            await RunDeliveryAnalysis(delivery, request, cancellationToken);
        });
    }

    private async Task RunDeliveryAnalysis(Delivery delivery, RunAnalyzerRequest request, CancellationToken cancellationToken)
    {
        var container = await CreateContainer($"analyzer-{request.AnalyzerId}-{delivery.Id}", cancellationToken);

        try
        {
            using var fleximetricsStream = File.OpenRead(FLEXIMETRICS_PATH);
            await CopyFileToContainer(container, fleximetricsStream, "fleximetrics.py");

            using var scriptStream = _fileStorage.GetAnalyzerScript(request.CourseId, request.AssignmentId, request.AnalyzerId);
            await CopyFileToContainer(container, scriptStream, "script.py");

            var deliveryDTO = DeliveryDTO.MapFrom(delivery);
            var deliveryJsonBytes = JsonSerializer.SerializeToUtf8Bytes(deliveryDTO);
            using var deliveryJsonStream = new MemoryStream(deliveryJsonBytes);
            await CopyFileToContainer(container, deliveryJsonStream, "delivery.json");

            await _dockerClient.Containers.StartContainerAsync(container, new ContainerStartParameters(), cancellationToken);

            await PrintLogs(container, cancellationToken);

            await _dockerClient.Containers.WaitContainerAsync(container, cancellationToken);

            await OnDeliveryAnalysisFinished(container);
        }
        finally
        {
            await _dockerClient.Containers.RemoveContainerAsync(container, new ContainerRemoveParameters(), CancellationToken.None);
        }
    }

    private async Task PrintLogs(string container, CancellationToken cancellationToken)
    {
        var logsStream = await _dockerClient.Containers.GetContainerLogsAsync(container, false, new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true,
            Follow = true
        }, cancellationToken);

        var (stdout, stderr) = await logsStream.ReadOutputToEndAsync(cancellationToken);

        if (stdout != string.Empty)
        {
            _logger.LogInformation(stdout);
        }
        if (stderr != string.Empty)
        {
            _logger.LogError(stderr);
        }
    }

    private async Task OnDeliveryAnalysisFinished(string container)
    {
        using var analysisStream = await CopyFileFromContainer(container, "analysis.json");
        var obj = await JsonSerializer.DeserializeAsync<object>(analysisStream);

        _logger.LogInformation(JsonSerializer.Serialize(obj));
    }

    private async Task<string> CreateContainer(string name, CancellationToken cancellationToken)
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

    private async Task CopyFileToContainer(string container, Stream stream, string fileName)
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

    private async Task<Stream> CopyFileFromContainer(string container, string path)
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
}
