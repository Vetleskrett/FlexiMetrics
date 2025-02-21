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
    Task<Result<IEnumerable<AssignmentResponse>>> GetAllByCourse(Guid courseId);
    Task<Result<IEnumerable<StudentAssignmentResponse>>> GetAllByStudentCourse(Guid studentId, Guid courseId);
    Task<Result<IEnumerable<StudentAssignmentResponse>>> GetAllByTeamCourse(Guid courseId, Guid teamId);
    Task<Result<AssignmentResponse>> GetById(Guid id);
    Task<Result<AssignmentResponse>> Create(CreateAssignmentRequest request);
    Task<Result<AssignmentResponse>> Update(UpdateAssignmentRequest request, Guid id);
    Task<Result<AssignmentResponse>> Publish(Guid id);
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

    public async Task<Result<IEnumerable<AssignmentResponse>>> GetAllByCourse(Guid courseId)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);
        if (course is null)
        {
            return Result<IEnumerable<AssignmentResponse>>.NotFound();
        }

        var assignments = await _dbContext.Assignments
            .Where(x => x.CourseId == courseId)
            .OrderBy(x => x.DueDate)
            .ToListAsync();

        return assignments.MapToResponse();
    }

    public async Task<Result<IEnumerable<StudentAssignmentResponse>>> GetAllByStudentCourse(Guid studentId, Guid courseId)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);
        if (course is null)
        {
            return Result<IEnumerable<StudentAssignmentResponse>>.NotFound();
        }

        var student = await _dbContext.Users.FindAsync(studentId);
        if (student is null)
        {
            return Result<IEnumerable<StudentAssignmentResponse>>.NotFound();
        }

        var courseStudent = await _dbContext.CourseStudents
            .FirstOrDefaultAsync(cs => cs.StudentId == studentId && cs.CourseId == courseId);

        if (courseStudent is null)
        {
            return new ValidationError("Student is not enrolled in the course").MapToResponse();
        }

        var assignments = await _dbContext.Assignments
            .Where(x => x.CourseId == courseId && x.Published == true)
            .OrderBy(x => x.DueDate)
            .Select(a => new StudentAssignmentResponse
            {
                Id = a.Id,
                Name = a.Name,
                DueDate = a.DueDate,
                CollaborationType = a.CollaborationType,
                CourseId = a.CourseId,
                IsDelivered = _dbContext.Deliveries
                        .Where(d => d.AssignmentId == a.Id)
                        .Any(d =>
                            (d.StudentId == studentId) ||
                            (d.Team != null && d.Team.Students.Any(s => s.Id == studentId))
                        ),
            })
            .ToListAsync();

        return assignments;
    }

    public async Task<Result<IEnumerable<StudentAssignmentResponse>>> GetAllByTeamCourse(Guid courseId, Guid teamId)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);
        if (course is null)
        {
            return Result<IEnumerable<StudentAssignmentResponse>>.NotFound();
        }

        var team = await _dbContext.Teams.FindAsync(teamId);
        if (team is null)
        {
            return Result<IEnumerable<StudentAssignmentResponse>>.NotFound();
        }

        var teamCourse = await _dbContext.Teams
            .FirstOrDefaultAsync(tc => tc.Id == teamId && tc.CourseId == courseId);

        if (teamCourse is null)
        {
            return new ValidationError("This team is not in the course").MapToResponse();
        }

        var assignments = await _dbContext.Assignments
            .Where(x => x.CourseId == courseId && x.Published == true && x.CollaborationType == CollaborationType.Teams)
            .OrderBy(x => x.DueDate)
            .Select(a => new StudentAssignmentResponse
            {
                Id = a.Id,
                Name = a.Name,
                DueDate = a.DueDate,
                CollaborationType = a.CollaborationType,
                CourseId = a.CourseId,
                IsDelivered = _dbContext.Deliveries
                        .Where(d => d.AssignmentId == a.Id)
                        .Any(d =>
                            (d.Team != null && d.Team.Id == teamId)
                        ),
            })
            .ToListAsync();

        return assignments;
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

    public async Task<Result<AssignmentResponse>> Publish(Guid id)
    {
        var assignment = await _dbContext.Assignments
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Id == id);
        if (assignment is null || assignment.Published)
        {
            return Result<AssignmentResponse>.NotFound();
        }

        assignment.Published = true;

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
