using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database;
using Api.Deliveries.Contracts;

namespace Api.Deliveries;

public interface IDeliveryService
{
    Task<IEnumerable<DeliveryResponse>> GetAll();
    Task<DeliveryResponse?> GetById(Guid id);
    Task<DeliveryResponse?> GetByStudentAssignment(Guid studentId, Guid assignmentId);
    Task<DeliveryResponse?> GetByTeamAssignment(Guid teamId, Guid assignmentId);
    Task<IEnumerable<DeliveryResponse>> GetAllByAssignment(Guid assignmentId);
    Task<Result<DeliveryResponse?, ValidationResponse>> Create(CreateDeliveryRequest request);
    Task<bool> DeleteById(Guid id);
}

public class DeliveryService : IDeliveryService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Delivery> _validator;

    public DeliveryService(AppDbContext dbContext, IValidator<Delivery> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<IEnumerable<DeliveryResponse>> GetAll()
    {
        var deliveries = await _dbContext.Deliveries
            .Include(d => d.Fields)
            .AsNoTracking()
            .ToListAsync();
        return deliveries.MapToResponse();
    }

    public async Task<DeliveryResponse?> GetById(Guid id)
    {
        var delivery = await _dbContext.Deliveries
            .Include(d => d.Fields)
            .FirstOrDefaultAsync(d => d.Id == id);
        return delivery?.MapToResponse();
    }

    public async Task<DeliveryResponse?> GetByStudentAssignment(Guid studentId, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return default;
        }

        var query = _dbContext.Deliveries
            .Include(d => d.Fields)
            .Where(d => d.AssignmentId == assignmentId);

        if (assignment.CollaborationType == CollaborationType.Individual)
        {
            var delivery = await query
                .FirstOrDefaultAsync(d => d.StudentId == studentId);
            return delivery?.MapToResponse();
        }
        else
        {
            var delivery = await query
                .FirstOrDefaultAsync(d => d.Team!.Students.Any(s => s.Id == studentId));
            return delivery?.MapToResponse();
        }
    }

    public async Task<DeliveryResponse?> GetByTeamAssignment(Guid teamId, Guid assignmentId)
    {
        var delivery = await _dbContext.Deliveries
            .Include(d => d.Fields)
            .FirstOrDefaultAsync(d => d.AssignmentId == assignmentId && d.TeamId == teamId);
        return delivery?.MapToResponse();
    }

    public async Task<IEnumerable<DeliveryResponse>> GetAllByAssignment(Guid assignmentId)
    {
        var deliveries = await _dbContext.Deliveries
            .Include(d => d.Fields)
            .Where(d => d.AssignmentId == assignmentId)
            .ToListAsync();

        return deliveries.MapToResponse();
    }

    public async Task<Result<DeliveryResponse?, ValidationResponse>> Create(CreateDeliveryRequest request)
    {
        var assignment = await _dbContext.Assignments
            .Include(a => a.Fields)
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId);

        if (assignment is null)
        {
            return default;
        }

        if (assignment.DueDate < DateTime.UtcNow)
        {
            return new ValidationError
            {
                PropertyName = nameof(assignment.DueDate),
                Message = "Cannot deliver after assignment due date"
            }.MapToResponse();
        }

        Delivery delivery;

        if (assignment.CollaborationType == CollaborationType.Teams)
        {
            var team = await _dbContext.Teams
                .Where(t => t.CourseId == assignment.CourseId)
                .Where(t => t.Students.Any(s => s.Id == request.StudentId))
                .FirstOrDefaultAsync();

            if (team is null)
            {
                return default;
            }

            delivery = request.MapToTeamDelivery(team.Id);
        }
        else
        {
            delivery = request.MapToStudentDelivery();
        }

        delivery.Assignment = assignment;

        var validationResult = await _validator.ValidateAsync(delivery);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        if (assignment.CollaborationType == CollaborationType.Individual)
        {
            await _dbContext.Deliveries
                .Where(d => d.AssignmentId == assignment.Id)
                .Where(d => d.StudentId == delivery.StudentId)
                .ExecuteDeleteAsync();
        }
        else
        {
            await _dbContext.Deliveries
                .Where(d => d.AssignmentId == assignment.Id)
                .Where(d => d.TeamId != null && d.TeamId == delivery.TeamId)
                .ExecuteDeleteAsync();
        }

        _dbContext.Deliveries.Add(delivery);
        await _dbContext.SaveChangesAsync();

        return delivery.MapToResponse();
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Deliveries.Where(x => x.Id == id).ExecuteDeleteAsync();
        return result > 0;
    }
}
