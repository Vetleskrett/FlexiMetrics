using Api.AssignmentFields.Contracts;
using Api.Validation;
using Database.Models;
using Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Api.Assignments;

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

    public AssignmentFieldService(AppDbContext dbContext, IValidator<Assignment> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
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

        var fieldsToBeUpdated = request.Fields
            .Where(f => f.Id.HasValue)
            .MapToAssignmentField(assignmentId);

        var fieldsToBeDeleted = assignment.Fields!.Where(field => !fieldsToBeUpdated.Any(f => f.Id == field.Id));
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

        await _dbContext.SaveChangesAsync();

        return assignment.Fields!.MapToResponse();
    }

    public async Task<Result> DeleteById(Guid id)
    {
        var deleted = await _dbContext.AssignmentFields.Where(x => x.Id == id).ExecuteDeleteAsync();
        return deleted > 0 ? Result.Success() : Result.NotFound();
    }
}
