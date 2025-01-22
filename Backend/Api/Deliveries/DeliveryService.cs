using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database;
using Api.Deliveries.Contracts;
using Movies.Api.Contracts.Responses;

namespace Api.Deliveries;

public interface IDeliveryService
{
    Task<IEnumerable<DeliveryResponse>> GetAll();
    Task<DeliveryResponse?> GetById(Guid id);
    Task<DeliveryResponse?> GetByStudentAssignment(Guid studentId, Guid assignmentId);
    Task<DeliveryResponse?> GetByTeamAssignment(Guid teamId, Guid assignmentId);
    Task<IEnumerable<DeliveryResponse>?> GetAllByAssignment(Guid assignmentId);
    Task<Result<DeliveryResponse, ValidationResponse>> Create(CreateDeliveryRequest request);
    Task<Result<DeliveryResponse?, ValidationResponse>> Update(UpdateDeliveryRequest request, Guid id);
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
        var delivery = await _dbContext.Deliveries
            .Include(d => d.Fields)
            .OfType<StudentDelivery>()
            .FirstOrDefaultAsync(d => d.AssignmentId == assignmentId && d.StudentId == studentId);
        return delivery?.MapToResponse();
    }

    public async Task<DeliveryResponse?> GetByTeamAssignment(Guid teamId, Guid assignmentId)
    {
        var delivery = await _dbContext.Deliveries
            .Include(d => d.Fields)
            .OfType<TeamDelivery>()
            .FirstOrDefaultAsync(d => d.AssignmentId == assignmentId && d.TeamId == teamId);
        return delivery?.MapToResponse();
    }

    public async Task<IEnumerable<DeliveryResponse>?> GetAllByAssignment(Guid assignmentId)
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
            var deliveries = await query.OfType<StudentDelivery>().ToListAsync();
            return deliveries.MapToResponse();
        }
        else
        {
            var deliveries = await query.OfType<TeamDelivery>().ToListAsync();
            return deliveries.MapToResponse();
        }
    }

    public async Task<Result<DeliveryResponse, ValidationResponse>> Create(CreateDeliveryRequest request)
    {
        var assignment = await _dbContext.Assignments
            .Include(a => a.Fields)
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId);

        if (assignment is null)
        {
            return new ValidationError
            {
                Message = "Could not find assignment",
                PropertyName = nameof(request.AssignmentId),
            }.MapToResponse();
        }

        var isIndividual = assignment.CollaborationType == CollaborationType.Individual;
        Delivery delivery = isIndividual ? request.MapToStudentDelivery() : request.MapToTeamDelivery();
        delivery.Assignment = assignment;

        var validationResult = await _validator.ValidateAsync(delivery);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Deliveries.Add(delivery);
        await _dbContext.SaveChangesAsync();

        return delivery.MapToResponse();
    }

    public async Task<Result<DeliveryResponse?, ValidationResponse>> Update(UpdateDeliveryRequest request, Guid id)
    {
        var delivery = await _dbContext.Deliveries
            .Include(d => d.Fields)
            .Include(d => d.Assignment)
            .ThenInclude(a => a!.Fields)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (delivery is null)
        {
            return default;
        }

        foreach (var newField in request.Fields)
        {
            var existingField = delivery.Fields!.FirstOrDefault(f => f.Id == newField.Id);
            if (existingField is null)
            {
                return default;
            }

            existingField.Value = newField.Value;
        }

        var validationResult = await _validator.ValidateAsync(delivery);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        await _dbContext.SaveChangesAsync();

        return delivery.MapToResponse();
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Deliveries.Where(x => x.Id == id).ExecuteDeleteAsync();
        return result > 0;
    }
}
