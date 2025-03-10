using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database;
using Api.Analyses.Contracts;
using Container;
using System.Runtime.CompilerServices;

namespace Api.Analyses;

public interface IAnalysisService
{
    Task<Result<IEnumerable<SlimAnalysisResponse>>> GetAll();
    Task<Result<AnalysisResponse>> GetById(Guid id);
    Task<Result<AnalyzerAnalysesResponse>> GetAllByAnalyzer(Guid analyzerId);
    IAsyncEnumerable<AnalysisStatusUpdateResponse> GetStatusEventsById(Guid id, CancellationToken cancellationToken);
    Task<Result> DeleteById(Guid id);
}

public class AnalysisService : IAnalysisService
{
    private readonly AppDbContext _dbContext;
    private readonly IAnalyzerExecutor _analyzerExecutor;

    public AnalysisService(AppDbContext dbContext, IAnalyzerExecutor analyzerExecutor)
    {
        _dbContext = dbContext;
        _analyzerExecutor = analyzerExecutor;
    }

    public async Task<Result<IEnumerable<SlimAnalysisResponse>>> GetAll()
    {
        var analyses = await _dbContext.Analyses
            .AsNoTracking()
            .OrderBy(a => a.AnalyzerId)
            .ThenByDescending(a => a.StartedAt)
            .ToListAsync();

        return analyses.MapToSlimResponse();
    }

    public async Task<Result<AnalysisResponse>> GetById(Guid id)
    {
        var analysis = await _dbContext.Analyses
            .Include(a => a.DeliveryAnalyses!)
            .ThenInclude(ae => ae.Fields)
            .Include(a => a.DeliveryAnalyses!)
            .ThenInclude(ae => ae.Team!)
            .ThenInclude(t => t.Students)
            .Include(a => a.DeliveryAnalyses!)
            .ThenInclude(ae => ae.Student)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (analysis is null)
        {
            return Result<AnalysisResponse>.NotFound();
        }

        return analysis.MapToResponse();
    }

    public async Task<Result<AnalyzerAnalysesResponse>> GetAllByAnalyzer(Guid analyzerId)
    {
        var analyses = await _dbContext.Analyses
            .AsNoTracking()
            .Where(a => a.AnalyzerId == analyzerId)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync();

        var latestId = analyses.FirstOrDefault()?.Id;

        var latest = latestId is null ? null :
            await _dbContext.Analyses
            .AsNoTracking()
            .Include(a => a.DeliveryAnalyses!)
            .ThenInclude(ae => ae.Fields)
            .Include(a => a.DeliveryAnalyses!)
            .ThenInclude(ae => ae.Team!)
            .ThenInclude(t => t.Students)
            .Include(a => a.DeliveryAnalyses!)
            .ThenInclude(ae => ae.Student)
            .FirstOrDefaultAsync(a => a.Id == latestId);

        return new AnalyzerAnalysesResponse
        {
            Analyses = analyses.MapToSlimResponse(),
            Latest = latest?.MapToResponse()
        };
    }

    /*
     * TODO:
     * - Split ContainerService into ContainerService and "AnalyzerRunnerService"
     * - Move GetStatusEventsById to analyzer folder (get status by analyzer id)
     * - Change AnalysisEntry to depend on student/team (also maybe change class name)
     * - Clean up state management in frontend page
     * - Fix sse bugs: When multiple readers, each reader does not recieve all events (maybe not use channels?)
     * - Consider making the Container project separate process in Aspire, with rabbitMq (or something) for communication
     * - Add .RequireAuthorization() on analysis endpoint group
    */

    public async IAsyncEnumerable<AnalysisStatusUpdateResponse> GetStatusEventsById(Guid id, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (await IsInactive(id))
        {
            yield break;
        }

        await foreach (var statusUpdate in _analyzerExecutor.GetStatusUpdates(id, cancellationToken))
        {
            var analysisEntry = await _dbContext.DeliveryAnalyses
                .Include(ae => ae.Fields)
                .Include(ae => ae.Team!)
                .ThenInclude(t => t.Students)
                .Include(ae => ae.Student)
                .FirstOrDefaultAsync(ae => ae.Id == statusUpdate.AnalysisEntryId, cancellationToken);

            yield return new AnalysisStatusUpdateResponse
            {
                AnalysisEntry = analysisEntry?.MapToResponse(),
                Logs = statusUpdate.Logs
            };

            if (await IsInactive(id))
            {
                yield break;
            }
        }
    }

    private async Task<bool> IsInactive(Guid id)
    {
        var analysis = await _dbContext.Analyses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        return analysis is null || analysis.Status == Database.Models.AnalysisStatus.Completed;
    }

    public async Task<Result> DeleteById(Guid id)
    {
        var analysis = await _dbContext.Analyses.FindAsync(id);
        if (analysis is null)
        {
            return Result.NotFound();
        }
        await _dbContext.Analyses.Where(x => x.Id == id).ExecuteDeleteAsync();
        return Result.Success();
    }
}
