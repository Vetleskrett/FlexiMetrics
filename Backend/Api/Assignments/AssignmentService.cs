using Api.AssignmentFields;
using Api.Assignments.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using FileStorage;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Assignments;

public interface IAssignmentService
{
    Task<Result<IEnumerable<AssignmentResponse>>> GetAll();
    Task<Result<IEnumerable<AssignmentResponse>>> GetAllByCourse(Guid courseId, bool includeDraft);
    Task<Result<AssignmentResponse>> GetById(Guid id);
    Task<Result<AssignmentResponse>> Create(CreateAssignmentRequest request);
    Task<Result<AssignmentResponse>> Update(UpdateAssignmentRequest request, Guid id);
    Task<Result> DeleteById(Guid id);
}

public class AssignmentService : IAssignmentService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Assignment> _validator;
    private readonly IFileStorage _fileStorage;

    public AssignmentService(AppDbContext dbContext, IValidator<Assignment> validator, IFileStorage fileStorage)
    {
        _dbContext = dbContext;
        _validator = validator;
        _fileStorage = fileStorage;
    }

    public async Task<Result<IEnumerable<AssignmentResponse>>> GetAll()
    {
        var assignments = await _dbContext.Assignments
            .AsNoTracking()
            .OrderBy(x => x.DueDate)
            .ToListAsync();
        return assignments.MapToResponse();
    }

    public async Task<Result<IEnumerable<AssignmentResponse>>> GetAllByCourse(Guid courseId, bool includeDraft)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);
        if (course is null)
        {
            return Result<IEnumerable<AssignmentResponse>>.NotFound();
        }

        var assignments = await _dbContext.Assignments
            .Where(a => a.CourseId == courseId)
            .Where(a => a.Published || includeDraft)
            .OrderBy(a => a.DueDate)
            .ToListAsync();

        return assignments.MapToResponse();
    }

    public async Task<Result<AssignmentResponse>> GetById(Guid id)
    {
        var assignment = await _dbContext.Assignments.FindAsync(id);
        if (assignment is null)
        {
            return Result<AssignmentResponse>.NotFound();
        }
        return assignment.MapToResponse();
    }

    public async Task<Result<AssignmentResponse>> Create(CreateAssignmentRequest request)
    {
        var assignment = request.MapToAssignment();
        var course = await _dbContext.Courses.FindAsync(assignment.CourseId);

        if (course is null)
        {
            return Result<AssignmentResponse>.NotFound();
        }

        var validationResult = await _validator.ValidateAsync(assignment);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Assignments.Add(assignment);
        _dbContext.AssignmentFields.AddRange(assignment.Fields!);

        await _dbContext.SaveChangesAsync();

        return assignment.MapToResponse();
    }

    public async Task<Result<AssignmentResponse>> Update(UpdateAssignmentRequest request, Guid id)
    {
        var assignment = await _dbContext.Assignments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (assignment is null)
        {
            return Result<AssignmentResponse>.NotFound();
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

    public async Task<Result> DeleteById(Guid id)
    {
        var assignment = await _dbContext.Assignments.FindAsync(id);
        if (assignment is null)
        {
            return Result.NotFound();
        }
        _fileStorage.DeleteAssignment(assignment.CourseId, assignment.Id);
        await _dbContext.Assignments.Where(x => x.Id == id).ExecuteDeleteAsync();
        return Result.Success();
    }
}
