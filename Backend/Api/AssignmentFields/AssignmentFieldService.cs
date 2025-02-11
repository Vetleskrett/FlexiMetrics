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
    Task<IEnumerable<AssignmentFieldResponse>> GetAll();
    Task<IEnumerable<AssignmentFieldResponse>?> GetAllByAssignment(Guid assignmentId);
    Task<Result<AssignmentFieldResponse?, ValidationResponse>> Create(CreateAssignmentFieldRequest request);
    Task<Result<IEnumerable<AssignmentFieldResponse>?, ValidationResponse>> Create(CreateAssignmentFieldsRequest requests);
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

    public async Task<IEnumerable<AssignmentFieldResponse>> GetAll()
    {
        var fields = await _dbContext.AssignmentFields
            .AsNoTracking()
            .ToListAsync();

        return fields.MapToResponse();
    }

    public async Task<IEnumerable<AssignmentFieldResponse>?> GetAllByAssignment(Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments
            .Include(a => a.Fields)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment is null)
        {
            return default;
        }

        return assignment.Fields!.MapToResponse();
    }

    public async Task<Result<AssignmentFieldResponse?, ValidationResponse>> Create(CreateAssignmentFieldRequest request)
    {
        var field = request.MapToAssignmentField();
        var assignment = await _dbContext.Assignments.FindAsync(field.AssignmentId);

        if (assignment is null)
        {
            return default;
        }

        var validationResult = await _validator.ValidateAsync(field);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.AssignmentFields.Add(field);
        await _dbContext.SaveChangesAsync();

        return field.MapToResponse();
    }

    public async Task<Result<IEnumerable<AssignmentFieldResponse>?, ValidationResponse>> Create(CreateAssignmentFieldsRequest requests)
    {
        var fields = requests.fields.MapToAssignmentField();
        foreach (var field in fields)
        {
            var assignment = await _dbContext.Assignments.FindAsync(field.AssignmentId);

            if (assignment is null)
            {
                return default;
            }

            var validationResult = await _validator.ValidateAsync(field);
            if (!validationResult.IsValid)
            {
                return validationResult.Errors.MapToResponse();
            }
        }

        _dbContext.AssignmentFields.AddRange(fields);
        await _dbContext.SaveChangesAsync();

        return fields.MapToResponse();
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
