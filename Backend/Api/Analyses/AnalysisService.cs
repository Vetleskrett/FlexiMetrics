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
            .Include(a => a.AnalysisEntries!)
            .ThenInclude(ae => ae.Fields)
            .Include(a => a.AnalysisEntries!)
            .ThenInclude(ae => ae.Team!)
            .ThenInclude(t => t.Students)
            .Include(a => a.AnalysisEntries!)
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
            .Include(a => a.AnalysisEntries!)
            .ThenInclude(ae => ae.Fields)
            .Include(a => a.AnalysisEntries!)
            .ThenInclude(ae => ae.Team!)
            .ThenInclude(t => t.Students)
            .Include(a => a.AnalysisEntries!)
            .ThenInclude(ae => ae.Student)
            .FirstOrDefaultAsync(a => a.Id == latestId);

        return new AnalyzerAnalysesResponse
        {
            Analyses = analyses.MapToSlimResponse(),
            Latest = latest?.MapToResponse()
        };
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
