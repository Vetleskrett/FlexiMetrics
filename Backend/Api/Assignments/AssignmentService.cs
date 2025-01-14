using Api.Utils;
using Api.Validation;
using Database;
using Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Assignments;

public interface IAssignmentService
{
    Task<IEnumerable<Assignment>> GetAll();
    Task<IEnumerable<Assignment>> GetAllByCourse(Guid courseId);
    Task<Assignment?> GetById(Guid id);
    Task<(Assignment, IEnumerable<AssignmentField>)?> GetByIdWithFields(Guid id);
    Task<Result<(Assignment, IEnumerable<AssignmentField>), ValidationFailed>> Create(Assignment assignment, List<AssignmentField> fields);
    Task<Result<(Assignment, IEnumerable<AssignmentField>)?, ValidationFailed>> Update(Assignment assignment, List<AssignmentField>? fields);
    Task<bool> DeleteById(Guid id);
}
public class AssignmentService : IAssignmentService
{
    protected readonly AppDbContext _dbContext;
    protected readonly IValidator<Assignment> _validator;

    public AssignmentService(AppDbContext dbContext, IValidator<Assignment> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }
    public async Task<IEnumerable<Assignment>> GetAll()
    {
        return await _dbContext.Assignments.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Assignment>> GetAllByCourse(Guid CourseId)
    {
        return await _dbContext.Assignments.Where(x => x.CourseId == CourseId).ToListAsync();
    }

    public async Task<Assignment?> GetById(Guid id)
    {
        return await _dbContext.Assignments.FindAsync(id);
    }

    public async Task<(Assignment, IEnumerable<AssignmentField>)?> GetByIdWithFields(Guid id)
    {
        var assignment = await _dbContext.Assignments.FindAsync(id);
        var fields = await _dbContext.AssignmentFields.AsNoTracking().Where(x => x.AssignmentId == id).ToListAsync();
        return assignment == null || fields == null ? null : (assignment, fields);
    }

    public async Task<Result<(Assignment, IEnumerable<AssignmentField>), ValidationFailed>> Create(Assignment assignment, List<AssignmentField> fields)
    {
        var validationResult = await _validator.ValidateAsync(assignment);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }
        try
        {
            _dbContext.Assignments.Add(assignment);
            fields.ForEach(x => x.AssignmentId = assignment.Id);
            _dbContext.AssignmentFields.AddRange(fields);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            // Find out what to do
            return new ValidationFailed([]);
        }

        return (assignment, fields);
    }

    public async Task<Result<(Assignment, IEnumerable<AssignmentField>)?, ValidationFailed>> Update(Assignment assignment, List<AssignmentField>? fields)
    {
        var validationResult = await _validator.ValidateAsync(assignment);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        var result = 0;
        try
        {
            _dbContext.Assignments.Update(assignment);
            var oldFields = await _dbContext.AssignmentFields.AsNoTracking().Where(x => x.AssignmentId == assignment.Id).ToListAsync();
            if (fields != null)
            {
                _dbContext.AssignmentFields.RemoveRange(oldFields);
                fields.ForEach(x => x.AssignmentId = assignment.Id);
                _dbContext.AssignmentFields.AddRange(fields);
            }
            else
            {
                fields = oldFields;
            }
            result = await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            // Find out what to do
            return new ValidationFailed([]);
        }
        return result > 0 ? (assignment, fields) : default;
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Assignments.Where(x => x.Id == id).ExecuteDeleteAsync();
        return result > 0;
    }
}
