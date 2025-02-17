using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database;
using Api.Feedbacks.Contracts;
using Api.Teams;

namespace Api.Feedbacks;

public interface IFeedbackService
{
    Task<IEnumerable<FeedbackResponse>> GetAll();
    Task<FeedbackResponse?> GetById(Guid id);
    Task<IEnumerable<FeedbackResponse>?> GetByAssignment(Guid assignmentId);
    Task<Result<FeedbackResponse?, ValidationResponse>> GetByStudentAssignment(Guid studentId, Guid assignmentId);
    Task<Result<FeedbackResponse?, ValidationResponse>> GetByTeamAssignment(Guid teamId, Guid assignmentId);
    Task<Result<FeedbackResponse, ValidationResponse>> Create(CreateFeedbackRequest request);
    Task<Result<FeedbackResponse?, ValidationResponse>> Update(UpdateFeedbackRequest request, Guid id);
    Task<bool> DeleteById(Guid id);
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

    public async Task<IEnumerable<FeedbackResponse>> GetAll()
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

    public async Task<FeedbackResponse?> GetById(Guid id)
    {
        var feedback = await _dbContext.Feedbacks
            .Include(f => f.Assignment)
            .FirstOrDefaultAsync(f => f.Id == id);
        return feedback?.MapToResponse();
    }

    public async Task<IEnumerable<FeedbackResponse>?> GetByAssignment(Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return default;
        }

        var feedbacks = await _dbContext.Feedbacks
            .Include(f => f.Assignment)
            .Where(f => f.AssignmentId == assignmentId)
            .ToListAsync();

        return feedbacks.MapToResponse();
    }

    public async Task<Result<FeedbackResponse?, ValidationResponse>> GetByStudentAssignment(Guid studentId, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return default;
        }

        var student = await _dbContext.Users.FindAsync(studentId);
        if (student is null)
        {
            return default;
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

        return feedback?.MapToResponse();
    }

    public async Task<Result<FeedbackResponse?, ValidationResponse>> GetByTeamAssignment(Guid teamId, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return default;
        }

        var team = await _dbContext.Teams.FindAsync(teamId);
        if (team is null)
        {
            return default;
        }

        if (team.CourseId != assignment.CourseId)
        {
            return new ValidationError("Team is not in the course").MapToResponse();
        }

        var feedback = await _dbContext.Feedbacks
            .Include(f => f.Assignment)
            .Where(f => f.AssignmentId == assignmentId && f.TeamId == teamId)
            .FirstOrDefaultAsync();

        return feedback?.MapToResponse();
    }

    public async Task<Result<FeedbackResponse, ValidationResponse>> Create(CreateFeedbackRequest request)
    {
        var assignment = await _dbContext.Assignments.FindAsync(request.AssignmentId);

        if (assignment is null)
        {
            return default;
        }

        if (assignment.CollaborationType == CollaborationType.Individual)
        {
            var student = await _dbContext.Users.FindAsync(request.StudentId);
            if (student is null)
            {
                return default;
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
                return default;
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

    public async Task<Result<FeedbackResponse?, ValidationResponse>> Update(UpdateFeedbackRequest request, Guid id)
    {
        var feedback = await _dbContext.Feedbacks
            .Include(d => d.Assignment)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (feedback is null)
        {
            return default;
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

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Feedbacks.Where(x => x.Id == id).ExecuteDeleteAsync();
        return result > 0;
    }
}
