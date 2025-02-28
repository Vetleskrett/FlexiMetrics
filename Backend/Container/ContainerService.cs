using Database;
using FileStorage;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Docker.DotNet;
using Docker.DotNet.Models;
using SharpCompress.Common;
using SharpCompress.Writers;
using SharpCompress.Readers;
using Microsoft.Extensions.Logging;
using System.Text;
namespace Container;

public interface IContainerService
{
    Task Initialize();
    Task StartAnalyzer(Guid id);
}

public class ContainerService : IContainerService
{
    private const string IMAGE = "python:3.13-slim";
    private const string FLEXIMETRICS_PATH = "../Container/fleximetrics.py";
    private const string WORKING_DIR = "/app";

    private readonly AppDbContext _dbContext;
    private readonly IFileStorage _fileStorage;
    private readonly IDockerClient _dockerClient;
    private readonly ILogger<ContainerService> _logger;

    public ContainerService(AppDbContext dbContext, IFileStorage fileStorage, IDockerClient dockerClient, ILogger<ContainerService> logger)
    {
        _dbContext = dbContext;
        _fileStorage = fileStorage;
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

    public async Task StartAnalyzer(Guid id)
    {
        var analyzer = await _dbContext.Analyzers
            .Include(a => a.Assignment)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (analyzer is null)
        {
            return;
        }

        // Get latest analysis
        // If status is running: return error

        // Create an analysis with status running

        // Add items to queue and return success

        // In background: process queue

        var deliveries = await _dbContext.Deliveries
            .Include(d => d.Student)
            .Include(d => d.Team)
            .ThenInclude(t => t!.Students)
            .Include(d => d.Fields)
            .Where(d => d.AssignmentId == analyzer.AssignmentId)
            .ToListAsync();

        await Parallel.ForEachAsync(deliveries, async (delivery, _) =>
        {
            var container = await CreateContainer($"analyzer-{id}-{delivery.Id}");

            try
            {
                using var fleximetricsStream = File.OpenRead(FLEXIMETRICS_PATH);
                await CopyFileToContainer(container, fleximetricsStream, "fleximetrics.py");

                using var scriptStream = _fileStorage.GetAnalyzerScript(analyzer.Assignment!.CourseId, analyzer.AssignmentId, analyzer.Id);
                await CopyFileToContainer(container, scriptStream, "script.py");

                var deliveryJson =
                """
                {
                    "student": {
                        "id": "123",
                        "name": "ola",
                        "email": "ola@ntnu.no"
                    },
                    "team": null,
                    "fields": {
                        "test_int": 5,
                        "test_str": "text"
                    }
                }
                """;
                var deliveryJsonBytes = Encoding.UTF8.GetBytes(deliveryJson);
                using var deliveryJsonStream = new MemoryStream(deliveryJsonBytes);
                await CopyFileToContainer(container, deliveryJsonStream, "delivery.json");

                await _dockerClient.Containers.StartContainerAsync(container, new ContainerStartParameters());

                await PrintLogs(container);

                await _dockerClient.Containers.WaitContainerAsync(container);

                await OnAnalyzerFinished(container);
            }
            finally
            {
                await _dockerClient.Containers.RemoveContainerAsync(container, new());
            }
        });
    }

    public async Task PrintLogs(string container)
    {
        var logsStream = await _dockerClient.Containers.GetContainerLogsAsync(container, false, new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true,
            Follow = true
        });

        var (stdout, stderr) = await logsStream.ReadOutputToEndAsync(CancellationToken.None);

        if (stdout != string.Empty)
        {
            _logger.LogInformation(stdout);
        }
        if (stderr != string.Empty)
        {
            _logger.LogError(stderr);
        }
    }

    public async Task OnAnalyzerFinished(string container)
    {
        using var analysisStream = await CopyFileFromContainer(container, "analysis.json");
        var obj = await JsonSerializer.DeserializeAsync<object>(analysisStream);

        _logger.LogInformation(JsonSerializer.Serialize(obj));
    }

    private async Task<string> CreateContainer(string name)
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
            }
        );
        return container.ID;
    }

    private async Task CopyFileToContainer(string container, Stream stream, string fileName)
    {
        using var tarStream = CreateTarArchive(stream, fileName);
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
        return ExtractTarArchive(response.Stream);
    }

    private Stream CreateTarArchive(Stream fileStream, string fileName)
    {
        var outputStream = new MemoryStream();
        using (var writer = WriterFactory.Open(outputStream, ArchiveType.Tar, CompressionType.None))
        {
            writer.Write(fileName, fileStream, DateTime.Now);
        }
        outputStream.Position = 0;
        return outputStream;
    }

    private Stream ExtractTarArchive(Stream tarStream)
    {
        var outputStream = new MemoryStream();
        using (var reader = ReaderFactory.Open(tarStream))
        {
            reader.MoveToNextEntry();
            reader.WriteEntryTo(outputStream);
        }
        outputStream.Position = 0;
        return outputStream;
    }
}
