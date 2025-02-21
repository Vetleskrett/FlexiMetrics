using Api.AssignmentFields.Contracts;
using Api.Validation;
using Database.Models;
using Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Api.Assignments;
using FileStorage;

namespace Api.AssignmentFields;

public interface IAssignmentFieldService
{
    Task<Result<IEnumerable<AssignmentFieldResponse>>> GetAll();
    Task<Result<IEnumerable<AssignmentFieldResponse>>> GetAllByAssignment(Guid assignmentId);
    Task<Result<IEnumerable<AssignmentFieldResponse>>> Update(UpdateAssignmentFieldsRequest request, Guid assignmentId);
    Task<Result> DeleteById(Guid id);
}

public class AssignmentFieldService : IAssignmentFieldService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Assignment> _validator;
    private IFileStorage _fileStorage;

    public AssignmentFieldService(AppDbContext dbContext, IValidator<Assignment> validator, IFileStorage fileStorage)
    {
        _dbContext = dbContext;
        _validator = validator;
        _fileStorage = fileStorage;
    }

    public async Task<Result<IEnumerable<AssignmentFieldResponse>>> GetAll()
    {
        var fields = await _dbContext.AssignmentFields
            .AsNoTracking()
            .ToListAsync();

        return fields.MapToResponse();
    }

    public async Task<Result<IEnumerable<AssignmentFieldResponse>>> GetAllByAssignment(Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments
            .Include(a => a.Fields)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment is null)
        {
            return Result<IEnumerable<AssignmentFieldResponse>>.NotFound();
        }

        return assignment.Fields!.MapToResponse();
    }

    public async Task<Result<IEnumerable<AssignmentFieldResponse>>> Update(UpdateAssignmentFieldsRequest request, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments
            .Include(a => a.Fields)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);
        if (assignment is null)
        {
            return Result<IEnumerable<AssignmentFieldResponse>>.NotFound();
        }

        var deliveries = await _dbContext.Deliveries
            .Include(a => a.Fields)
            .Where(d => d.AssignmentId == assignmentId)
            .ToListAsync();

        var fieldsToBeUpdated = request.Fields
            .Where(f => f.Id.HasValue)
            .MapToAssignmentField(assignmentId);

        var fieldsToBeDeleted = assignment.Fields!
            .Where(field => !fieldsToBeUpdated.Any(f => f.Id == field.Id))
            .ToList();

        _dbContext.AssignmentFields.RemoveRange(fieldsToBeDeleted);

        foreach (var field in fieldsToBeUpdated)
        {
            var exisingField = assignment.Fields!.FirstOrDefault(f => f.Id == field.Id);
            if (exisingField is null)
            {
                return Result<IEnumerable<AssignmentFieldResponse>>.NotFound();
            }

            exisingField.Name = field.Name;
            exisingField.Type = field.Type;
            exisingField.Min = field.Min;
            exisingField.Max = field.Max;
            exisingField.Regex = field.Regex;
            exisingField.SubType = field.SubType;
        }

        var fieldsToBeCreated = request.Fields
            .Where(f => !f.Id.HasValue)
            .MapToAssignmentField(assignmentId);
        _dbContext.AssignmentFields.AddRange(fieldsToBeCreated);

        var validationResult = await _validator.ValidateAsync(assignment);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        foreach (var delivery in deliveries)
        {
            foreach (var deliveryField in delivery.Fields!)
            {
                if (fieldsToBeDeleted.Any(f => f.Id == deliveryField.AssignmentFieldId))
                {
                    _fileStorage.DeleteDeliveryField(assignment.CourseId, assignmentId, delivery.Id, deliveryField.Id);
                }
            }
        }

        await _dbContext.SaveChangesAsync();

        return assignment.Fields!.MapToResponse();
    }

    public async Task<Result> DeleteById(Guid id)
    {
        var deliveryFields = await _dbContext.DeliveryFields
            .Include(f => f.Delivery!)
            .ThenInclude(d => d.Assignment)
            .Where(f => f.AssignmentFieldId == id)
            .ToListAsync();

        foreach (var deliveryField in deliveryFields)
        {
            _fileStorage.DeleteDeliveryField(deliveryField.Delivery!.Assignment!.CourseId, deliveryField.Delivery!.AssignmentId, deliveryField.Delivery.Id, deliveryField.Id);
        }

        var deleted = await _dbContext.AssignmentFields.Where(x => x.Id == id).ExecuteDeleteAsync();
        return deleted > 0 ? Result.Success() : Result.NotFound();
    }
}
