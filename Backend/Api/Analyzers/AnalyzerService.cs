using Api.Analyses;
using Api.Analyses.Contracts;
using Api.Analyzers.Contracts;
using Api.Common;
using Api.Validation;
using Container.Contracts;
using Database;
using Database.Models;
using FileStorage;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;

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
    Task<Result<IEnumerable<AnalyzerLogResponse>>> GetLogsById(Guid analyzerId);
    Task<Result> StartAction(AnalyzerActionRequest request, Guid id);
    Task<Result> DeleteById(Guid id);
}

public class AnalyzerService : IAnalyzerService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Analyzer> _validator;
    private readonly IFileStorage _fileStorage;
    private readonly IBus _bus;

    public AnalyzerService(AppDbContext dbContext, IValidator<Analyzer> validator, IFileStorage fileStorage, IBus bus)
    {
        _dbContext = dbContext;
        _validator = validator;
        _fileStorage = fileStorage;
        _bus = bus;
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

        await _bus.Publish(new BuildAnalyzerRequest(analyzer.Id));

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

        if (analyzer.State != AnalyzerState.Standby)
        {
            return new ValidationError("Cannot edit analyzer in current state").MapToResponse();
        }

        analyzer.Name = request.Name;
        analyzer.FileName = request.FileName;

        var rebuild = false;
        if (request.Requirements != analyzer.Requirements)
        {
            analyzer.Requirements = request.Requirements;
            rebuild = true;
        }
        if (analyzer.AptPackages != request.AptPackages)
        {
            analyzer.AptPackages = request.AptPackages;
            rebuild = true;
        }

        if (rebuild)
        {
            analyzer.State = AnalyzerState.Building;
        }

        var validationResult = await _validator.ValidateAsync(analyzer);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        await _dbContext.SaveChangesAsync();

        if (rebuild)
        {
            await _bus.Publish(new BuildAnalyzerRequest(analyzer.Id));
        }

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

        if (analyzer.State == AnalyzerState.Running)
        {
            return new ValidationError("Cannot edit script while analyzer is running").MapToResponse();
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
            var script = await _fileStorage.GetAnalyzerScript
            (
                analyzer!.Assignment!.CourseId,
                analyzer!.AssignmentId,
                analyzer.Id
            );

            return new FileResponse
            {
                Stream = new MemoryStream(Encoding.UTF8.GetBytes(script)),
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

    public async Task<Result<IEnumerable<AnalyzerLogResponse>>> GetLogsById(Guid analyzerId)
    {
        var analyzer = await _dbContext.Analyzers.FindAsync(analyzerId);

        if (analyzer is null)
        {
            return Result<IEnumerable<AnalyzerLogResponse>>.NotFound();
        }

        var logs = await _dbContext.AnalyzerLogs
            .Where(al => al.AnalyzerId == analyzerId)
            .OrderBy(al => al.Timestamp)
            .ToListAsync();

        return logs.MapToResponse();
    }

    public async Task<Result> StartAction(AnalyzerActionRequest request, Guid id)
    {
        var analyzer = await _dbContext.Analyzers
            .Include(a => a.Assignment)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (analyzer is null)
        {
            return Result.NotFound();
        }

        if (request.Action == AnalyzerAction.Run)
        {
            return await RunAnalyzer(analyzer);
        }
        else
        {
            return await CancelAnalyzer(analyzer);
        }
    }

    private async Task<Result> CancelAnalyzer(Analyzer analyzer)
    {
        if (analyzer.State != AnalyzerState.Running)
        {
            return new ValidationError("Analyzer not running").MapToResponse();
        }

        await _dbContext.Analyses
            .Where(a => a.AnalyzerId == analyzer.Id)
            .Where(a => a.Status == AnalysisStatus.Running)
            .ExecuteUpdateAsync(x => x.SetProperty(a => a.Status, AnalysisStatus.Canceled));

        analyzer.State = AnalyzerState.Standby;
        await _dbContext.SaveChangesAsync();

        await _bus.Publish(new CancelAnalyzerRequest(analyzer.Id));

        return Result.Success();
    }

    private async Task<Result> RunAnalyzer(Analyzer analyzer)
    {
        if (analyzer.State != AnalyzerState.Standby)
        {
            return new ValidationError("Cannot run analyzer in the current state").MapToResponse();
        }

        analyzer.State = AnalyzerState.Running;

        var analysis = new Analysis
        {
            Id = Guid.NewGuid(),
            Status = AnalysisStatus.Running,
            StartedAt = DateTime.UtcNow,
            CompletedAt = null,
            AnalyzerId = analyzer.Id,
            AnalysisEntries = [],
        };
        _dbContext.Analyses.Add(analysis);

        var entries = await GetEntries(analyzer, analysis);
        _dbContext.AnalysisEntries.AddRange(entries);

        await _dbContext.SaveChangesAsync();

        await _bus.Publish(new RunAnalyzerRequest(analyzer.Assignment!.CourseId, analyzer.AssignmentId, analyzer.Id, analysis.Id));

        return Result.Success();
    }

    private async Task<List<AnalysisEntry>> GetEntries(Analyzer analyzer, Analysis analysis)
    {
        var assignment = await _dbContext.Assignments.FindAsync(analyzer.AssignmentId);

        if (assignment!.CollaborationType == CollaborationType.Individual)
        {
            return await _dbContext.CourseStudents
                .Where(cs => cs.CourseId == assignment.CourseId)
                .Select(cs => cs.Student!)
                .Select(student =>
                    new AnalysisEntry
                    {
                        Id = Guid.NewGuid(),
                        AnalysisId = analysis.Id,
                        StudentId = student.Id,
                        TeamId = null,
                        Fields = new(),
                        CompletedAt = null
                    }
                )
                .ToListAsync();
        }
        else
        {
            return await _dbContext.Teams
                .Where(t => t.CourseId == assignment.CourseId)
                .Select(team =>
                    new AnalysisEntry
                    {
                        Id = Guid.NewGuid(),
                        AnalysisId = analysis.Id,
                        StudentId = null,
                        TeamId = team.Id,
                        Fields = new(),
                        CompletedAt = null
                    }
                )
                .ToListAsync();
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
        await _bus.Publish(new DeleteAnalyzerRequest(id));

        await _dbContext.Analyzers.Where(x => x.Id == id).ExecuteDeleteAsync();
        return Result.Success();
    }
}
