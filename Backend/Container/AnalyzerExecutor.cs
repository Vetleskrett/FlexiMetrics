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

public partial class AnalyzerExecutor : IAnalyzerExecutor
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

    private async Task<List<AssignmentEntry>> GetEntries(RunAnalyzerRequest request)
    {
        var assignment = await _dbContext.Assignments.FindAsync(request.AssignmentId);

        if (assignment!.CollaborationType == CollaborationType.Individual)
        {
            return await _dbContext.CourseStudents
                .Where(cs => cs.CourseId == request.CourseId)
                .Select(cs => cs.Student!)
                .Select(student =>
                    new AssignmentEntry
                    {
                        Student = student,
                        Team = null,
                        Delivery = _dbContext.Deliveries.FirstOrDefault(d =>
                            d.StudentId == student.Id &&
                            d.AssignmentId == request.AssignmentId
                        )
                    }
                )
                .ToListAsync();
        }
        else
        {
            return await _dbContext.Teams
                .Where(t => t.CourseId == request.CourseId)
                .Include(t => t.Students)
                .Select(team =>
                    new AssignmentEntry
                    {
                        Student = null,
                        Team = team,
                        Delivery = _dbContext.Deliveries.FirstOrDefault(d =>
                            d.TeamId == team.Id &&
                            d.AssignmentId == request.AssignmentId
                        )
                    }
                )
                .ToListAsync();
        }
    }

    public async Task RunAnalyzer(RunAnalyzerRequest request, CancellationToken cancellationToken)
    {
        await _dbContext.Analyses
            .Where(a => a.Id == request.AnalysisId)
            .ExecuteUpdateAsync(setter => setter.SetProperty(a => a.Status, AnalysisStatus.Running));

        var entries = await GetEntries(request);

        await Parallel.ForEachAsync(entries, cancellationToken, async (entry, cancellationToken) =>
        {
            await RunAnalysisEntry(entry, request, cancellationToken);
        });

        await _dbContext.Analyses
            .Where(a => a.Id == request.AnalysisId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(a => a.Status, AnalysisStatus.Completed)
                .SetProperty(a => a.CompletedAt, DateTime.UtcNow)
            );
    }

    private async Task RunAnalysisEntry(AssignmentEntry entry, RunAnalyzerRequest request, CancellationToken cancellationToken)
    {
        var container = await _containerService.CreateContainer($"analyzer-{request.AnalyzerId}-{entry.Id}", cancellationToken);

        try
        {
            using var fleximetricsStream = File.OpenRead(FLEXIMETRICS_PATH);
            await _containerService.CopyFileToContainer(container, fleximetricsStream, "fleximetrics.py");

            using var scriptStream = _fileStorage.GetAnalyzerScript(request.CourseId, request.AssignmentId, request.AnalyzerId);
            await _containerService.CopyFileToContainer(container, scriptStream, "script.py");

            var entryDTO = AssignmentEntryDTO.MapFrom(entry);
            var entryJsonBytes = JsonSerializer.SerializeToUtf8Bytes(entryDTO);
            using var entryJsonStream = new MemoryStream(entryJsonBytes);
            await _containerService.CopyFileToContainer(container, entryJsonStream, "input.json");

            await _containerService.StartContainer(container, cancellationToken);

            var logs = await _containerService.GetLogs(container, cancellationToken);

            await _containerService.WaitForContainerCompletion(container, cancellationToken);

            var analysisEntry = await OnAnalysisEntryFinished(container, entry, request);

            await _analysisStatusChannel.Writer.WriteAsync(new(request.AnalysisId, logs), cancellationToken);
        }
        finally
        {
            await _containerService.RemoveContainer(container);
        }
    }

    private class OutputField
    {
        public required AnalysisFieldType Type { get; init; }
        public required object Value { get; init; }
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private async Task<AnalysisEntry> OnAnalysisEntryFinished(string container, AssignmentEntry entry, RunAnalyzerRequest request)
    {
        using var outputStream = await _containerService.CopyFileFromContainer(container, "output.json");
        var outputFields = await JsonSerializer.DeserializeAsync<Dictionary<string, OutputField>>(outputStream, _jsonOptions);

        var analysisEntry = new AnalysisEntry
        {
            Id = Guid.NewGuid(),
            AnalysisId = request.AnalysisId,
            StudentId = entry.Student?.Id,
            TeamId = entry.Team?.Id,
            Fields = []
        };

        var analysisFields = outputFields!.Select(pair =>
            new AnalysisField
            {
                Id = Guid.NewGuid(),
                AnalysisEntryId = analysisEntry.Id,
                Name = pair.Key,
                Type = pair.Value.Type,
                Value = pair.Value.Value,
            }
        ).ToList();

        await _dbLock.WaitAsync();
        try
        {
            _dbContext.DeliveryAnalyses.Add(analysisEntry);
            _dbContext.AnalysisFields.AddRange(analysisFields);
            await _dbContext.SaveChangesAsync();
        }
        finally
        {
            _dbLock.Release();
        }

        return analysisEntry;
    }
}
