using Api.Assignments.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Movies.Api.Contracts.Responses;

namespace Api.Assignments;

public interface IAssignmentService
{
    Task<IEnumerable<AssignmentResponse>> GetAll();
    Task<IEnumerable<AssignmentResponse>> GetAllByCourse(Guid courseId);
    Task<AssignmentResponse?> GetById(Guid id);
    Task<Result<AssignmentResponse, ValidationResponse>> Create(CreateAssignmentRequest request);
    Task<Result<AssignmentResponse?, ValidationResponse>> Update(UpdateAssignmentRequest request, Guid id);
    Task<bool> DeleteById(Guid id);
}

public class AssignmentService : IAssignmentService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Assignment> _validator;

    public AssignmentService(AppDbContext dbContext, IValidator<Assignment> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<IEnumerable<AssignmentResponse>> GetAll()
    {
        var assignments = await _dbContext.Assignments
            .AsNoTracking()
            .ToListAsync();
        return assignments.MapToResponse();
    }

    public async Task<IEnumerable<AssignmentResponse>> GetAllByCourse(Guid CourseId)
    {
        var assignments = await _dbContext.Assignments
            .Where(x => x.CourseId == CourseId)
            .ToListAsync();
        return assignments.MapToResponse();
    }

    public async Task<AssignmentResponse?> GetById(Guid id)
    {
        var assignment = await _dbContext.Assignments.FindAsync(id);
        return assignment?.MapToResponse();
    }

    public async Task<Result<AssignmentResponse, ValidationResponse>> Create(CreateAssignmentRequest request)
    {
        var assignment = request.MapToAssignment();

        var validationResult = await _validator.ValidateAsync(assignment);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.AssignmentFields.AddRange(assignment.Fields!);
        _dbContext.Assignments.Add(assignment);

        await _dbContext.SaveChangesAsync();

        return assignment.MapToResponse();
    }

    public async Task<Result<AssignmentResponse?, ValidationResponse>> Update(UpdateAssignmentRequest request, Guid id)
    {
        var assignment = await _dbContext.Assignments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (assignment is null)
        {
            return default;
        }

        assignment = request.MapToAssignment(assignment.Id, assignment.CourseId);

        var validationResult = await _validator.ValidateAsync(assignment);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Assignments.Update(assignment);
        await _dbContext.SaveChangesAsync();

        return assignment.MapToResponse();
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Assignments.Where(x => x.Id == id).ExecuteDeleteAsync();
        return result > 0;
    }
}
