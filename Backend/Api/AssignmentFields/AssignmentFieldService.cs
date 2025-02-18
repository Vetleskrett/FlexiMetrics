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
    Task<Result<AssignmentFieldResponse>> Create(CreateAssignmentFieldRequest request);
    Task<Result<IEnumerable<AssignmentFieldResponse>>> Create(CreateAssignmentFieldsRequest requests);
    Task<Result<AssignmentFieldResponse>> Update(UpdateAssignmentFieldRequest request, Guid id);
    Task<Result> DeleteById(Guid id);
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

    public async Task<Result<AssignmentFieldResponse>> Create(CreateAssignmentFieldRequest request)
    {
        var field = request.MapToAssignmentField();
        var assignment = await _dbContext.Assignments.FindAsync(field.AssignmentId);

        if (assignment is null)
        {
            return Result<AssignmentFieldResponse>.NotFound();
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

    public async Task<Result<IEnumerable<AssignmentFieldResponse>>> Create(CreateAssignmentFieldsRequest requests)
    {
        var fields = requests.Fields.MapToAssignmentField();
        foreach (var field in fields)
        {
            var assignment = await _dbContext.Assignments.FindAsync(field.AssignmentId);

            if (assignment is null)
            {
                return Result<IEnumerable<AssignmentFieldResponse>>.NotFound();
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

    public async Task<Result<AssignmentFieldResponse>> Update(UpdateAssignmentFieldRequest request, Guid id)
    {
        var field = await _dbContext.AssignmentFields
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (field is null)
        {
            return Result<AssignmentFieldResponse>.NotFound();
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

    public async Task<Result> DeleteById(Guid id)
    {
        var deleted = await _dbContext.AssignmentFields.Where(x => x.Id == id).ExecuteDeleteAsync();
        return deleted > 0 ? Result.Success() : Result.NotFound();
    }
}
