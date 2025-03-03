using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database;
using Api.Analyzers.Contracts;
using FileStorage;
using Api.Common;
using System.Net.Mime;
using Container;

namespace Api.Analyzers;

public interface IAnalyzerService
{
    Task<Result<IEnumerable<AnalyzerResponse>>> GetAll();
    Task<Result<AnalyzerResponse>> GetById(Guid id);
    Task<Result<IEnumerable<AnalyzerResponse>>> GetAllByAssignment(Guid assignmentId);
    Task<Result<AnalyzerResponse>> Create(CreateAnalyzerRequest request);
    Task<Result<AnalyzerResponse>> Update(UpdateAnalyzerRequest request, Guid id);
    Task<Result> UploadScript(IFormFile script, Guid analyzerFieldId);
    Task<Result<FileResponse>> DownloadScript(Guid analyzerFieldId);
    Task<Result> StartAction(AnalyzerActionRequest request, Guid id);
    Task<Result> DeleteById(Guid id);
}

public class AnalyzerService : IAnalyzerService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Analyzer> _validator;
    private readonly IFileStorage _fileStorage;
    private readonly IContainerService _containerService;

    public AnalyzerService(AppDbContext dbContext, IValidator<Analyzer> validator, IFileStorage fileStorage, IContainerService containerService)
    {
        _dbContext = dbContext;
        _validator = validator;
        _fileStorage = fileStorage;
        _containerService = containerService;
    }

    public async Task<Result<IEnumerable<AnalyzerResponse>>> GetAll()
    {
        var analyzers = await _dbContext.Analyzers
            .AsNoTracking()
            .ToListAsync();

        return analyzers.MapToResponse();
    }

    public async Task<Result<AnalyzerResponse>> GetById(Guid id)
    {
        var analyzer = await _dbContext.Analyzers
            .FirstOrDefaultAsync(a => a.Id == id);

        if (analyzer is null)
        {
            return Result<AnalyzerResponse>.NotFound();
        }

        return analyzer.MapToResponse();
    }

    public async Task<Result<IEnumerable<AnalyzerResponse>>> GetAllByAssignment(Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<IEnumerable<AnalyzerResponse>>.NotFound();
        }

        var analyzers = await _dbContext.Analyzers
            .Where(d => d.AssignmentId == assignmentId)
            .ToListAsync();

        return analyzers.MapToResponse();
    }

    public async Task<Result<AnalyzerResponse>> Create(CreateAnalyzerRequest request)
    {
        var assignment = await _dbContext.Assignments
            .Include(a => a.Fields)
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId);
        if (assignment is null)
        {
            return Result<AnalyzerResponse>.NotFound();
        }

        var analyzer = request.MapToAnalyzer();

        var validationResult = await _validator.ValidateAsync(analyzer);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Analyzers.Add(analyzer);
        await _dbContext.SaveChangesAsync();

        return analyzer.MapToResponse();
    }

    public async Task<Result<AnalyzerResponse>> Update(UpdateAnalyzerRequest request, Guid id)
    {
        var analyzer = await _dbContext.Analyzers
            .FirstOrDefaultAsync(d => d.Id == id);

        if (analyzer is null)
        {
            return Result<AnalyzerResponse>.NotFound();
        }

        analyzer.Name = request.Name;
        analyzer.FileName = request.FileName;

        var validationResult = await _validator.ValidateAsync(analyzer);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        await _dbContext.SaveChangesAsync();

        return analyzer.MapToResponse();
    }

    public async Task<Result> UploadScript(IFormFile script, Guid id)
    {
        var analyzer = await _dbContext.Analyzers
            .Include(a => a.Assignment)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (analyzer is null)
        {
            return Result.NotFound();
        }

        if (script.FileName != analyzer.FileName)
        {
            return new ValidationError("Invalid filename").MapToResponse();
        }

        try
        {
            await _fileStorage.WriteAnalyzerScript
            (
                analyzer!.Assignment!.CourseId,
                analyzer!.AssignmentId,
                analyzer.Id,
                script.OpenReadStream()
            );
        }
        catch (Exception e)
        {
            return new ValidationError($"Could not save file: {e.Message}").MapToResponse();
        }

        return Result.Success();
    }

    public async Task<Result<FileResponse>> DownloadScript(Guid id)
    {
        var analyzer = await _dbContext.Analyzers
            .Include(a => a.Assignment)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (analyzer is null)
        {
            return Result<FileResponse>.NotFound();
        }

        try
        {
            var stream = _fileStorage.GetAnalyzerScript
            (
                analyzer!.Assignment!.CourseId,
                analyzer!.AssignmentId,
                analyzer.Id
            );

            return new FileResponse
            {
                Stream = stream,
                Metadata = new FileMetadata
                {
                    FileName = analyzer.FileName,
                    ContentType = MediaTypeNames.Text.Plain
                }
            };
        }
        catch (Exception e)
        {
            return new ValidationError($"Could not read file: {e.Message}").MapToResponse();
        }
    }

    public async Task<Result> StartAction(AnalyzerActionRequest request, Guid id)
    {
        if (request.Action == AnalyzerAction.Run)
        {
            var analyzer = await _dbContext.Analyzers
                .Include(a => a.Assignment)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (analyzer is null)
            {
                return Result.NotFound();
            }

            var latestAnalysis = await _dbContext.Analyses
                .Where(a => a.AnalyzerId == id)
                .OrderByDescending(a => a.StartedAt)
                .FirstOrDefaultAsync();

            if (latestAnalysis is not null && latestAnalysis.AnalysisStatus != AnalysisStatus.Completed)
            {
                return new ValidationError("Analyzer already running").MapToResponse();
            }

            var analysis = new Analysis
            {
                Id = Guid.NewGuid(),
                AnalysisStatus = AnalysisStatus.Started,
                StartedAt = DateTime.UtcNow,
                CompletedAt = null,
                AnalyzerId = id,
                DeliveryAnalyses = []
            };
            _dbContext.Analyses.Add(analysis);
            await _dbContext.SaveChangesAsync();

            await _containerService.StartAnalyzer(analyzer.Assignment!.CourseId, analyzer.AssignmentId, analyzer.Id, analysis.Id);

            return Result.Success();
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public async Task<Result> DeleteById(Guid id)
    {
        var analyzer = await _dbContext.Analyzers
            .Include(d => d.Assignment)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (analyzer is null)
        {
            return Result.NotFound();
        }

        _fileStorage.DeleteAnalyzer(analyzer.Assignment!.CourseId, analyzer.AssignmentId, analyzer.Id);

        await _dbContext.Analyzers.Where(x => x.Id == id).ExecuteDeleteAsync();
        return Result.Success();
    }
}
