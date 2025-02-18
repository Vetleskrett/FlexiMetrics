using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database;
using Api.Deliveries.Contracts;

namespace Api.Deliveries;

public interface IDeliveryService
{
    Task<Result<IEnumerable<DeliveryResponse>>> GetAll();
    Task<Result<DeliveryResponse>> GetById(Guid id);
    Task<Result<DeliveryResponse>> GetByStudentAssignment(Guid studentId, Guid assignmentId);
    Task<Result<DeliveryResponse>> GetByTeamAssignment(Guid teamId, Guid assignmentId);
    Task<Result<IEnumerable<DeliveryResponse>>> GetAllByAssignment(Guid assignmentId);
    Task<Result<DeliveryResponse>> Create(CreateDeliveryRequest request);
    Task<Result> DeleteById(Guid id);
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

    public async Task<Result<IEnumerable<DeliveryResponse>>> GetAll()
    {
        var deliveries = await _dbContext.Deliveries
            .Include(d => d.Fields!.OrderBy(f => f.AssignmentFieldId))
            .OrderBy(d => d.Student!.Email)
            .ThenBy(d => d.Team!.TeamNr)
            .AsNoTracking()
            .ToListAsync();

        return deliveries.MapToResponse();
    }

    public async Task<Result<DeliveryResponse>> GetById(Guid id)
    {
        var delivery = await _dbContext.Deliveries
            .Include(d => d.Fields!.OrderBy(f => f.AssignmentFieldId))
            .FirstOrDefaultAsync(d => d.Id == id);

        if (delivery is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        return delivery.MapToResponse();
    }

    public async Task<Result<DeliveryResponse>> GetByStudentAssignment(Guid studentId, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        var student = await _dbContext.Users.FindAsync(studentId);
        if (student is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        var courseStudent = await _dbContext.CourseStudents
            .FirstOrDefaultAsync(cs => cs.CourseId == assignment.CourseId && cs.StudentId == studentId);
        if (courseStudent is null)
        {
            return new ValidationError("Student is not enrolled in the course").MapToResponse();
        }

        var query = _dbContext.Deliveries
            .Include(d => d.Fields!.OrderBy(f => f.AssignmentFieldId))
            .Where(d => d.AssignmentId == assignmentId);

        Delivery? delivery;
        if (assignment.CollaborationType == CollaborationType.Individual)
        {
            delivery = await query
                .FirstOrDefaultAsync(d => d.StudentId == studentId);
        }
        else
        {
            delivery = await query
                .FirstOrDefaultAsync(d => d.Team!.Students.Any(s => s.Id == studentId));
        }

        if (delivery is null)
        {
            return Result<DeliveryResponse>.NoContent();
        }
        return delivery.MapToResponse();
    }

    public async Task<Result<DeliveryResponse>> GetByTeamAssignment(Guid teamId, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        var team = await _dbContext.Teams.FindAsync(teamId);
        if (team is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        if (team.CourseId != assignment.CourseId)
        {
            return new ValidationError("Team is not in the course").MapToResponse();
        }

        if (assignment.CollaborationType == CollaborationType.Individual)
        {
            return new ValidationError("Assignment is individual").MapToResponse();
        }

        var delivery = await _dbContext.Deliveries
            .Include(d => d.Fields!.OrderBy(f => f.AssignmentFieldId))
            .FirstOrDefaultAsync(d => d.AssignmentId == assignmentId && d.TeamId == teamId);

        if (delivery is null)
        {
            return Result<DeliveryResponse>.NoContent();
        }
        return delivery.MapToResponse();
    }

    public async Task<Result<IEnumerable<DeliveryResponse>>> GetAllByAssignment(Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<IEnumerable<DeliveryResponse>>.NotFound();
        }

        var deliveries = await _dbContext.Deliveries
            .Include(d => d.Fields!.OrderBy(f => f.AssignmentFieldId))
            .OrderBy(d => d.Student!.Email)
            .ThenBy(d => d.Team!.TeamNr)
            .Where(d => d.AssignmentId == assignmentId)
            .ToListAsync();

        return deliveries.MapToResponse();
    }

    public async Task<Result<DeliveryResponse>> Create(CreateDeliveryRequest request)
    {
        var assignment = await _dbContext.Assignments
            .Include(a => a.Fields)
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId);
        if (assignment is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        var student = await _dbContext.Users.FindAsync(request.StudentId);
        if (student is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        var courseStudent = await _dbContext.CourseStudents
            .FirstOrDefaultAsync(cs => cs.CourseId == assignment.CourseId && cs.StudentId == student.Id);
        if (courseStudent is null)
        {
            return new ValidationError("Student is not enrolled in the course").MapToResponse();
        }

        if (assignment.DueDate < DateTime.UtcNow)
        {
            return new ValidationError("Cannot deliver after assignment due date").MapToResponse();
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
                return Result<DeliveryResponse>.NotFound();
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

    public async Task<Result> DeleteById(Guid id)
    {
        var deleted = await _dbContext.Deliveries.Where(x => x.Id == id).ExecuteDeleteAsync();
        return deleted > 0 ? Result.Success() : Result.NotFound();
    }
}
