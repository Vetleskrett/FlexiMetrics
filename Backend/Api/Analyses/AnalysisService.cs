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
    private readonly IContainerService _containerService;

    public AnalysisService(AppDbContext dbContext, IContainerService containerService)
    {
        _dbContext = dbContext;
        _containerService = containerService;
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
            .ThenInclude(da => da.Fields)
            .Include(a => a.DeliveryAnalyses!)
            .ThenInclude(da => da.Delivery!.Team!)
            .ThenInclude(t => t.Students)
            .Include(a => a.DeliveryAnalyses!)
            .ThenInclude(da => da.Delivery!.Student)
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
            .ThenInclude(da => da.Fields)
            .Include(a => a.DeliveryAnalyses!)
            .ThenInclude(da => da.Delivery!.Team!)
            .ThenInclude(t => t.Students)
            .Include(a => a.DeliveryAnalyses!)
            .ThenInclude(da => da.Delivery!.Student)
            .FirstOrDefaultAsync(a => a.Id == latestId);

        return new AnalyzerAnalysesResponse
        {
            Analyses = analyses.MapToSlimResponse(),
            Latest = latest?.MapToResponse()
        };
    }

    public async IAsyncEnumerable<AnalysisStatusUpdateResponse> GetStatusEventsById(Guid id, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (await IsInactive(id))
        {
            yield break;
        }

        await foreach (var statusUpdate in _containerService.GetStatusUpdates(id, cancellationToken))
        {
            var deliveryAnalysis = await _dbContext.DeliveryAnalyses
                .Include(da => da.Fields)
                .Include(da => da.Delivery!.Team!)
                .ThenInclude(t => t.Students)
                .Include(da => da.Delivery!.Student)
                .FirstOrDefaultAsync(da => da.Id == statusUpdate.DeliveryAnalysisId, cancellationToken);

            yield return new AnalysisStatusUpdateResponse
            {
                DeliveryAnalysis = deliveryAnalysis?.MapToResponse(),
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
