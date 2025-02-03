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
    Task<FeedbackResponse?> GetByStudentAssignment(Guid studentId, Guid assignmentId);
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

    public async Task<FeedbackResponse?> GetByStudentAssignment(Guid studentId, Guid assignmentId)
    {
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
        var assignment = await _dbContext.Deliveries
            .Where(d => d.Id == request.DeliveryId)
            .Select(d => d.Assignment)
            .FirstOrDefaultAsync();

        if (assignment is null)
        {
            return default;
        }

        if (assignment.DueDate > DateTime.UtcNow)
        {
            return new ValidationError
            {
                PropertyName = nameof(assignment.DueDate),
                Message = "Cannot give feedback before assignment due date"
            }.MapToResponse();
        }

        var feedback = request.MapToFeedback();
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
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (feedback is null)
        {
            return default;
        }

        feedback = request.MapToFeedback(id);

        var validationResult = await _validator.ValidateAsync(feedback);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Feedbacks.Update(feedback);
        var numEntriesUpdated = await _dbContext.SaveChangesAsync();

        return numEntriesUpdated > 0 ? feedback.MapToResponse() : default;
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Feedbacks.Where(x => x.Id == id).ExecuteDeleteAsync();
        return result > 0;
    }
}
