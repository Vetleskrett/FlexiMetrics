using Database;
using FileStorage;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
using Database.Models;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;
using Container.Models;

namespace Container;

public interface IAnalyzerExecutor
{
    Task StartAnalyzer(Guid courseId, Guid assignmentId, Guid analyzerId, Guid analysisId);
    IAsyncEnumerable<AnalysisStatusUpdate> GetStatusUpdates(Guid analysisId, CancellationToken cancellationToken);
    Task RunAnalyzer(RunAnalyzerRequest request, CancellationToken cancellationToken);
}

public class AnalyzerExecutor : IAnalyzerExecutor
{
    private const string FLEXIMETRICS_PATH = "../Container/scripts/fleximetrics.py";

    private readonly AppDbContext _dbContext;
    private readonly IContainerService _containerService;
    private readonly IFileStorage _fileStorage;
    private readonly Channel<RunAnalyzerRequest> _runAnalyzerChannel;
    private readonly Channel<AnalysisStatusUpdate> _analysisStatusChannel;
    private readonly SemaphoreSlim _dbLock = new(1);

    public AnalyzerExecutor(AppDbContext dbContext, IContainerService containerService, IFileStorage fileStorage, Channel<RunAnalyzerRequest> runAnalyzerChannel, Channel<AnalysisStatusUpdate> analysisStatusChannel)
    {
        _dbContext = dbContext;
        _containerService = containerService;
        _fileStorage = fileStorage;
        _runAnalyzerChannel = runAnalyzerChannel;
        _analysisStatusChannel = analysisStatusChannel;
    }


    public async Task StartAnalyzer(Guid courseId, Guid assignmentId, Guid analyzerId, Guid analysisId)
    {
        await _runAnalyzerChannel.Writer.WriteAsync(new(courseId, assignmentId, analyzerId, analysisId));
    }

    public async IAsyncEnumerable<AnalysisStatusUpdate> GetStatusUpdates(Guid analysisId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var statusUpdate = await _analysisStatusChannel.Reader.ReadAsync(cancellationToken);

            if (statusUpdate.AnalysisId == analysisId)
            {
                yield return statusUpdate;
            }
        }
    }

    public async Task RunAnalyzer(RunAnalyzerRequest request, CancellationToken cancellationToken)
    {
        await _dbContext.Analyses
            .Where(a => a.Id == request.AnalysisId)
            .ExecuteUpdateAsync(setter => setter.SetProperty(a => a.Status, AnalysisStatus.Running));

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

        await _dbContext.Analyses
            .Where(a => a.Id == request.AnalysisId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(a => a.Status, AnalysisStatus.Completed)
                .SetProperty(a => a.CompletedAt, DateTime.UtcNow)
            );
    }

    private async Task RunDeliveryAnalysis(Delivery delivery, RunAnalyzerRequest request, CancellationToken cancellationToken)
    {
        var container = await _containerService.CreateContainer($"analyzer-{request.AnalyzerId}-{delivery.Id}", cancellationToken);

        try
        {
            using var fleximetricsStream = File.OpenRead(FLEXIMETRICS_PATH);
            await _containerService.CopyFileToContainer(container, fleximetricsStream, "fleximetrics.py");

            using var scriptStream = _fileStorage.GetAnalyzerScript(request.CourseId, request.AssignmentId, request.AnalyzerId);
            await _containerService.CopyFileToContainer(container, scriptStream, "script.py");

            var deliveryDTO = DeliveryDTO.MapFrom(delivery);
            var deliveryJsonBytes = JsonSerializer.SerializeToUtf8Bytes(deliveryDTO);
            using var deliveryJsonStream = new MemoryStream(deliveryJsonBytes);
            await _containerService.CopyFileToContainer(container, deliveryJsonStream, "delivery.json");

            await _containerService.StartContainer(container, cancellationToken);

            var logs = await _containerService.GetLogs(container, cancellationToken);

            await _containerService.WaitForContainerCompletion(container, cancellationToken);

            var deliveryAnalysis = await OnDeliveryAnalysisFinished(container, delivery, request);

            await _analysisStatusChannel.Writer.WriteAsync(new(request.AnalysisId, deliveryAnalysis.Id, logs), cancellationToken);
        }
        finally
        {
            await _containerService.RemoveContainer(container);
        }
    }

    private class AnalysisField
    {
        public required AnalysisFieldType Type { get; init; }
        public required object Value { get; init; }
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private async Task<DeliveryAnalysis> OnDeliveryAnalysisFinished(string container, Delivery delivery, RunAnalyzerRequest request)
    {
        using var analysisStream = await _containerService.CopyFileFromContainer(container, "analysis.json");
        var fields = await JsonSerializer.DeserializeAsync<Dictionary<string, AnalysisField>>(analysisStream, _jsonOptions);

        var deliveryAnalysis = new DeliveryAnalysis
        {
            Id = Guid.NewGuid(),
            AnalysisId = request.AnalysisId,
            DeliveryId = delivery.Id,
            Fields = []
        };

        var deliveryAnalysisFields = fields!.Select(pair =>
            new DeliveryAnalysisField
            {
                Id = Guid.NewGuid(),
                DeliveryAnalysisId = deliveryAnalysis.Id,
                Name = pair.Key,
                Type = pair.Value.Type,
                Value = pair.Value.Value,
            }
        ).ToList();

        await _dbLock.WaitAsync();
        try
        {
            _dbContext.DeliveryAnalyses.Add(deliveryAnalysis);
            _dbContext.DeliveryAnalysisFields.AddRange(deliveryAnalysisFields);
            await _dbContext.SaveChangesAsync();
        }
        finally
        {
            _dbLock.Release();
        }

        return deliveryAnalysis;
    }
}
