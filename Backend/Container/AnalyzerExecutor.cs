using Container.Models;
using Database;
using Database.Models;
using FileStorage;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Container;

public interface IAnalyzerExecutor
{
    Task RunAnalyzer(RunAnalyzerRequest request, CancellationToken cancellationToken);
}

public partial class AnalyzerExecutor : IAnalyzerExecutor
{
    private readonly AppDbContext _dbContext;
    private readonly IContainerService _containerService;
    private readonly IFileStorage _fileStorage;
    private readonly SemaphoreSlim _dbLock = new(1);
    private readonly IBus _bus;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true
    };

    public AnalyzerExecutor(AppDbContext dbContext, IContainerService containerService, IFileStorage fileStorage, IBus bus)
    {
        _dbContext = dbContext;
        _containerService = containerService;
        _fileStorage = fileStorage;
        _bus = bus;
    }

    public async Task RunAnalyzer(RunAnalyzerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Analyses
                .Where(a => a.Id == request.AnalysisId)
                .ExecuteUpdateAsync(setter => setter.SetProperty(a => a.Status, AnalysisStatus.Running), cancellationToken);

            await _bus.Publish(new AnalyzerStatusUpdate(request.AnalyzerId), cancellationToken);

            var entries = await GetEntries(request);

            var analyzer = await _dbContext.Analyzers.FindAsync(request.AnalyzerId);

            var script = await _fileStorage.GetAnalyzerScript(request.CourseId, request.AssignmentId, request.AnalyzerId);

            await _containerService.CreateImage(analyzer!.Id, script, analyzer.Requirements, cancellationToken);

            await Parallel.ForEachAsync(entries, cancellationToken, async (entry, cancellationToken) =>
            {
                await RunAnalysisEntry(entry, request, cancellationToken);
            });

            await _dbContext.Analyses
                .Where(a => a.Id == request.AnalysisId)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(a => a.Status, AnalysisStatus.Completed)
                    .SetProperty(a => a.CompletedAt, DateTime.UtcNow)
                , cancellationToken);

            await _bus.Publish(new AnalyzerStatusUpdate(request.AnalyzerId), cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch
        {
            await _dbContext.Analyses
                .Where(a => a.Id == request.AnalysisId)
                .ExecuteUpdateAsync(setter => setter.SetProperty(a => a.Status, AnalysisStatus.Failed), cancellationToken);

            await _bus.Publish(new AnalyzerStatusUpdate(request.AnalyzerId), cancellationToken);

            throw;
        }
    }

    private async Task RunAnalysisEntry(AssignmentEntry entry, RunAnalyzerRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var container = await _containerService.CreateContainer(request.AnalyzerId, entry.Id, CancellationToken.None);

        cancellationToken.Register(async () =>
        {
            await _containerService.RemoveContainer(container);
        });

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entryDTO = AssignmentEntryDTO.MapFrom(entry);
            var entryJson = JsonSerializer.Serialize(entryDTO, _jsonOptions);
            await _containerService.CopyFileToContainer(container, entryJson, "input.json");

            await _containerService.StartContainer(container, cancellationToken);

            await _containerService.WaitForContainerCompletion(container, cancellationToken);

            await OnAnalysisEntryFinished(container, entry, request, cancellationToken);

            await _bus.Publish(new AnalyzerStatusUpdate(request.AnalyzerId), cancellationToken);
        }
        finally
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await _containerService.RemoveContainer(container);
            }
        }
    }

    private async Task OnAnalysisEntryFinished(string container, AssignmentEntry entry, RunAnalyzerRequest request, CancellationToken cancellationToken)
    {
        var (logInformation, logError) = await _containerService.GetLogs(container, cancellationToken);

        var analysisEntry = new AnalysisEntry
        {
            Id = Guid.NewGuid(),
            AnalysisId = request.AnalysisId,
            StudentId = entry.Student?.Id,
            TeamId = entry.Team?.Id,
            Fields = [],
            LogInformation = logInformation,
            LogError = logError,
            CompletedAt = DateTime.UtcNow
        };

        using var outputStream = await _containerService.CopyFileFromContainer(container, "output.json");

        if (outputStream is not null)
        {
            var outputFields = await JsonSerializer.DeserializeAsync<Dictionary<string, OutputField>>(outputStream, _jsonOptions, cancellationToken);
            analysisEntry.Fields = outputFields!.Select(pair =>
                new AnalysisField
                {
                    Id = Guid.NewGuid(),
                    AnalysisEntryId = analysisEntry.Id,
                    Name = pair.Key,
                    Type = pair.Value.Type,
                    SubType = pair.Value.SubType,
                    Value = pair.Value.Value,
                }
            ).ToList();
        }

        await _dbLock.WaitAsync(cancellationToken);
        try
        {
            _dbContext.AnalysisEntries.Add(analysisEntry);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _dbLock.Release();
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
                        Delivery = _dbContext.Deliveries
                            .Include(d => d.Fields!)
                            .ThenInclude(f => f.AssignmentField)
                            .FirstOrDefault(d =>
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
                        Delivery = _dbContext.Deliveries
                            .Include(d => d.Fields!)
                            .ThenInclude(f => f.AssignmentField)
                            .FirstOrDefault(d =>
                                d.TeamId == team.Id &&
                                d.AssignmentId == request.AssignmentId
                            )
                    }
                )
                .ToListAsync();
        }
    }
}
