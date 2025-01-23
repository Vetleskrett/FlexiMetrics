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
    Task<IEnumerable<AssignmentFieldResponse>?> GetAllByAssignment(Guid assignmentId);
    Task<Result<AssignmentFieldResponse, ValidationResponse>> Create(CreateAssignmentFieldRequest request);
    Task<Result<AssignmentFieldResponse?, ValidationResponse>> Update(UpdateAssignmentFieldRequest request, Guid id);
    Task<bool> DeleteById(Guid id);
}

public class AssignmentFieldService : IAssignmentFieldService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<AssignmentField> _validator;

    public AssignmentFieldService(AppDbContext dbContext, IValidator<AssignmentField> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<IEnumerable<AssignmentFieldResponse>?> GetAllByAssignment(Guid assignmentId)
    {
        var fields = await _dbContext.AssignmentFields
            .AsNoTracking()
            .Where(f => f.AssignmentId == assignmentId)
            .ToListAsync();

        return fields.MapToResponse();
    }

    public async Task<Result<AssignmentFieldResponse, ValidationResponse>> Create(CreateAssignmentFieldRequest request)
    {
        var field = request.MapToAssignmentField();

        var validationResult = await _validator.ValidateAsync(field);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.AssignmentFields.Add(field);
        await _dbContext.SaveChangesAsync();

        return field.MapToResponse();
    }

    public async Task<Result<AssignmentFieldResponse?, ValidationResponse>> Update(UpdateAssignmentFieldRequest request, Guid id)
    {
        var field = await _dbContext.AssignmentFields
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (field is null)
        {
            return default;
        }

        field = request.MapToAssignmentField(id, field.AssignmentId);

        var validationResult = await _validator.ValidateAsync(field);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.AssignmentFields.Update(field);
        await _dbContext.SaveChangesAsync();

        return field.MapToResponse();
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.AssignmentFields.Where(x => x.Id == id).ExecuteDeleteAsync();
        return result > 0;
    }
}
