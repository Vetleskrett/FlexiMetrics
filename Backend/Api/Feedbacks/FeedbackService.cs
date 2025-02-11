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
    Task<Result<FeedbackResponse?, ValidationResponse>> GetByStudentAssignment(Guid studentId, Guid assignmentId);
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
        var feedbacks = await _dbContext.Feedbacks.AsNoTracking().ToListAsync();
        return feedbacks.MapToResponse();
    }

    public async Task<FeedbackResponse?> GetById(Guid id)
    {
        var feedback = await _dbContext.Feedbacks.FindAsync(id);
        return feedback?.MapToResponse();
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
            .Where(f => f.Delivery!.AssignmentId == assignmentId)
            .Where(f =>
                (f.Delivery!.StudentId == studentId) ||
                (f.Delivery.Team != null && f.Delivery.Team.Students.Any(s => s.Id == studentId))
            )
            .FirstOrDefaultAsync();

        return feedback?.MapToResponse();
    }

    public async Task<Result<FeedbackResponse, ValidationResponse>> Create(CreateFeedbackRequest request)
    {
        var delivery = await _dbContext.Deliveries
            .Include(d => d.Assignment)
            .FirstOrDefaultAsync(d => d.Id == request.DeliveryId);

        if (delivery is null)
        {
            return default;
        }

        if (delivery.Assignment!.DueDate > DateTime.UtcNow)
        {
            return new ValidationError("Cannot give feedback before assignment due date").MapToResponse();
        }

        var feedback = request.MapToFeedback();
        feedback.Delivery = delivery;
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
            .Include(f => f.Delivery!)
            .ThenInclude(d => d.Assignment)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (feedback is null)
        {
            return default;
        }

        var delivery = feedback.Delivery;
        feedback = request.MapToFeedback(id, feedback.DeliveryId);
        feedback.Delivery = delivery;

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
