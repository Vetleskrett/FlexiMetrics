using Api.Analyses.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Analyses;

public interface IAnalysisService
{
    Task<Result<IEnumerable<SlimAnalysisResponse>>> GetAll();
    Task<Result<AnalysisResponse>> GetById(Guid id);
    Task<Result<AnalyzerAnalysesResponse>> GetAllByAnalyzer(Guid analyzerId);
    Task<Result<IEnumerable<StudentAnalysisResponse>>> GetStudentAssignmentAnalyses(Guid studentId, Guid assignmentId);
    Task<Result<IEnumerable<StudentAnalysisResponse>>> GetTeamAssignmentAnalyses(Guid teamId, Guid assignmentId);
    Task<Result> DeleteById(Guid id);
}

public class AnalysisService : IAnalysisService
{
    private readonly AppDbContext _dbContext;

    public AnalysisService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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

    public async Task<Result<IEnumerable<StudentAnalysisResponse>>> GetStudentAssignmentAnalyses(Guid studentId, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<IEnumerable<StudentAnalysisResponse>>.NotFound();
        }

        var student = await _dbContext.Users.FindAsync(studentId);
        if (student is null)
        {
            return Result<IEnumerable<StudentAnalysisResponse>>.NotFound();
        }

        var courseStudent = await _dbContext.CourseStudents
            .FirstOrDefaultAsync(cs => cs.CourseId == assignment.CourseId && cs.StudentId == studentId);
        if (courseStudent is null)
        {
            return new ValidationError("Student is not enrolled in the course").MapToResponse();
        }

        if (assignment.CollaborationType == CollaborationType.Individual)
        {
            var analyses = await _dbContext.AnalysisEntries
                .Include(ae => ae.Fields)
                .Include(ae => ae.Analysis!)
                .ThenInclude(a => a.Analyzer)
                .Where(ae => ae.Analysis!.Analyzer!.AssignmentId == assignmentId)
                .Where(ae => ae.StudentId == studentId)
                .GroupBy(ae => ae.Analysis!.AnalyzerId)
                .Select(group => group.OrderByDescending(ae => ae.CompletedAt).First())
                .ToListAsync();

            return analyses.MapToStudentResponse();
        }
        else
        {
            var analyses = await _dbContext.AnalysisEntries
                .Include(ae => ae.Fields)
                .Include(ae => ae.Analysis!)
                .ThenInclude(a => a.Analyzer)
                .Where(ae => ae.Analysis!.Analyzer!.AssignmentId == assignmentId)
                .Where(ae => ae.Team!.Students.Any(s => s.Id == studentId))
                .GroupBy(ae => ae.Analysis!.AnalyzerId)
                .Select(group => group.OrderByDescending(ae => ae.CompletedAt).First())
                .ToListAsync();

            return analyses.MapToStudentResponse();
        }
    }

    public async Task<Result<IEnumerable<StudentAnalysisResponse>>> GetTeamAssignmentAnalyses(Guid teamId, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<IEnumerable<StudentAnalysisResponse>>.NotFound();
        }

        var team = await _dbContext.Teams.FindAsync(teamId);
        if (team is null)
        {
            return Result<IEnumerable<StudentAnalysisResponse>>.NotFound();
        }

        if (team.CourseId != assignment.CourseId)
        {
            return new ValidationError("Team is not in the course").MapToResponse();
        }

        var analyses = await _dbContext.AnalysisEntries
            .Include(ae => ae.Fields)
            .Include(ae => ae.Analysis!)
            .ThenInclude(a => a.Analyzer)
            .Where(ae => ae.Analysis!.Analyzer!.AssignmentId == assignmentId)
            .Where(ae => ae.TeamId == teamId)
            .GroupBy(ae => ae.Analysis!.AnalyzerId)
            .Select(group => group.OrderByDescending(ae => ae.CompletedAt).First())
            .ToListAsync();

        return analyses.MapToStudentResponse();
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
