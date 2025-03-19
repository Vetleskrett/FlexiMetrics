using Api.Feedbacks.Contracts;
using Api.Teams;
using Api.Validation;
using Database;
using Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Feedbacks;

public interface IFeedbackService
{
    Task<Result<IEnumerable<FeedbackResponse>>> GetAll();
    Task<Result<FeedbackResponse>> GetById(Guid id);
    Task<Result<IEnumerable<FeedbackResponse>>> GetByAssignment(Guid assignmentId);
    Task<Result<FeedbackResponse>> GetByStudentAssignment(Guid studentId, Guid assignmentId);
    Task<Result<FeedbackResponse>> GetByTeamAssignment(Guid teamId, Guid assignmentId);
    Task<Result<FeedbackResponse>> Create(CreateFeedbackRequest request);
    Task<Result<FeedbackResponse>> Update(UpdateFeedbackRequest request, Guid id);
    Task<Result> DeleteById(Guid id);
}

public class FeedbackService : IFeedbackService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Feedback> _validator;

    public FeedbackService(AppDbContext dbContext, IValidator<Feedback> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result<IEnumerable<FeedbackResponse>>> GetAll()
    {
        var feedbacks = await _dbContext.Feedbacks
            .AsNoTracking()
            .OrderBy(f => f.Assignment!.Name)
            .ThenBy(f => f.Assignment!.GradingType)
            .ThenBy(f => f.StudentId)
            .ThenBy(f => f.TeamId)
            .ToListAsync();
        return feedbacks.MapToResponse();
    }

    public async Task<Result<FeedbackResponse>> GetById(Guid id)
    {
        var feedback = await _dbContext.Feedbacks
            .Include(f => f.Assignment)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (feedback is null)
        {
            return Result<FeedbackResponse>.NotFound();
        }

        return feedback.MapToResponse();
    }

    public async Task<Result<IEnumerable<FeedbackResponse>>> GetByAssignment(Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<IEnumerable<FeedbackResponse>>.NotFound();
        }

        var feedbacks = await _dbContext.Feedbacks
            .Include(f => f.Assignment)
            .Where(f => f.AssignmentId == assignmentId)
            .ToListAsync();

        return feedbacks.MapToResponse();
    }

    public async Task<Result<FeedbackResponse>> GetByStudentAssignment(Guid studentId, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<FeedbackResponse>.NotFound();
        }

        var student = await _dbContext.Users.FindAsync(studentId);
        if (student is null)
        {
            return Result<FeedbackResponse>.NotFound();
        }

        var courseStudent = await _dbContext.CourseStudents
            .FirstOrDefaultAsync(cs => cs.CourseId == assignment.CourseId && cs.StudentId == studentId);
        if (courseStudent is null)
        {
            return new ValidationError("Student is not enrolled in the course").MapToResponse();
        }

        var feedback = await _dbContext.Feedbacks
            .Include(f => f.Assignment)
            .Where(f => f.AssignmentId == assignmentId)
            .Where(f =>
                (f.StudentId == studentId) ||
                (f.Team != null && f.Team.Students.Any(s => s.Id == studentId))
            )
            .FirstOrDefaultAsync();

        if (feedback is null)
        {
            return Result<FeedbackResponse>.NoContent();
        }

        return feedback.MapToResponse();
    }

    public async Task<Result<FeedbackResponse>> GetByTeamAssignment(Guid teamId, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<FeedbackResponse>.NotFound();
        }

        var team = await _dbContext.Teams.FindAsync(teamId);
        if (team is null)
        {
            return Result<FeedbackResponse>.NotFound();
        }

        if (team.CourseId != assignment.CourseId)
        {
            return new ValidationError("Team is not in the course").MapToResponse();
        }

        var feedback = await _dbContext.Feedbacks
            .Include(f => f.Assignment)
            .Where(f => f.AssignmentId == assignmentId && f.TeamId == teamId)
            .FirstOrDefaultAsync();

        if (feedback is null)
        {
            return Result<FeedbackResponse>.NoContent();
        }

        return feedback.MapToResponse();
    }

    public async Task<Result<FeedbackResponse>> Create(CreateFeedbackRequest request)
    {
        var assignment = await _dbContext.Assignments.FindAsync(request.AssignmentId);

        if (assignment is null)
        {
            return Result<FeedbackResponse>.NotFound();
        }

        if (assignment.CollaborationType == CollaborationType.Individual)
        {
            var student = await _dbContext.Users.FindAsync(request.StudentId);
            if (student is null)
            {
                return Result<FeedbackResponse>.NotFound();
            }

            var courseStudent = await _dbContext.CourseStudents
                .FirstOrDefaultAsync(cs => cs.CourseId == assignment.CourseId && cs.StudentId == student.Id);
            if (courseStudent is null)
            {
                return new ValidationError("Student is not enrolled in the course").MapToResponse();
            }
        }
        else
        {
            var team = await _dbContext.Teams.FindAsync(request.TeamId);
            if (team is null)
            {
                return Result<FeedbackResponse>.NotFound();
            }
            if (team.CourseId != assignment.CourseId)
            {
                return new ValidationError("Team is not in the course").MapToResponse();
            }
        }

        if (assignment.DueDate > DateTime.UtcNow)
        {
            return new ValidationError("Cannot give feedback before assignment due date").MapToResponse();
        }

        var feedback = request.MapToFeedback();
        feedback.Assignment = assignment;
        var validationResult = await _validator.ValidateAsync(feedback);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Feedbacks.Add(feedback);
        await _dbContext.SaveChangesAsync();

        return feedback.MapToResponse();
    }

    public async Task<Result<FeedbackResponse>> Update(UpdateFeedbackRequest request, Guid id)
    {
        var feedback = await _dbContext.Feedbacks
            .Include(d => d.Assignment)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (feedback is null)
        {
            return Result<FeedbackResponse>.NotFound();
        }

        var assignment = feedback.Assignment;
        feedback = request.MapToFeedback(id, feedback.AssignmentId, feedback.StudentId, feedback.TeamId);
        feedback.Assignment = assignment;

        var validationResult = await _validator.ValidateAsync(feedback);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Feedbacks.Update(feedback);
        await _dbContext.SaveChangesAsync();

        return feedback.MapToResponse();
    }

    public async Task<Result> DeleteById(Guid id)
    {
        var deleted = await _dbContext.Feedbacks.Where(x => x.Id == id).ExecuteDeleteAsync();
        return deleted > 0 ? Result.Success() : Result.NotFound();
    }
}
