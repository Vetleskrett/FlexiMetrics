using Api.Utils;
using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database;
using Api.Courses.Contracts;

namespace Api.Courses;

public interface ICourseService
{
    Task<IEnumerable<CourseResponse>> GetAll();
    Task<IEnumerable<CourseResponse>> GetAllByTeacherId(Guid teacherId);
    Task<IEnumerable<CourseResponse>> GetAllByStudentId(Guid studentId);
    Task<CourseFullResponse?> GetById(Guid id);
    Task<Result<CourseResponse, ValidationFailed>> Create(CreateCourseRequest request);
    Task<Result<CourseResponse?, ValidationFailed>> Update(UpdateCourseRequest request, Guid id);
    Task<bool> DeleteById(Guid id);
}

public class CourseService : ICourseService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Course> _validator;

    public CourseService(AppDbContext dbContext, IValidator<Course> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<IEnumerable<CourseResponse>> GetAll()
    {
        var courses = await _dbContext.Courses.AsNoTracking().ToListAsync();
        return courses.MapToResponse();
    }

    public async Task<IEnumerable<CourseResponse>> GetAllByTeacherId(Guid teacherId)
    {
        var courses = await _dbContext.Courses
            .Where(c => c.Teachers!.Any(s => s.Id == teacherId))
            .AsNoTracking()
            .ToListAsync();
        return courses.MapToResponse();
    }

    public async Task<IEnumerable<CourseResponse>> GetAllByStudentId(Guid studentId)
    {
        var courses = await _dbContext.Courses
            .Where(c => c.Students!.Any(s => s.Id == studentId))
            .AsNoTracking()
            .ToListAsync();
        return courses.MapToResponse();
    }

    public async Task<CourseFullResponse?> GetById(Guid id)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Teachers)
            .Select(course =>
                new CourseFullResponse
                {
                    Id = course.Id,
                    Code = course.Code,
                    Name = course.Name,
                    Year = course.Year,
                    Semester = course.Semester,
                    NumStudents = course.Students!.Count,
                    NumTeams = course.Teams!.Count,
                    Teachers = course.Teachers!.Select(t => new TeacherResponse
                    {
                        Id = t.Id,
                        Email = t.Email,
                        Name = t.Name,
                    }).ToList(),
                }
            )
            .FirstOrDefaultAsync(c => c.Id == id);

        return course;
    }

    public async Task<Result<CourseResponse, ValidationFailed>> Create(CreateCourseRequest request)
    {
        var course = request.MapToCourse();
        var validationResult = await _validator.ValidateAsync(course);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        _dbContext.Courses.Add(course);
        await _dbContext.SaveChangesAsync();

        return course.MapToResponse();
    }

    public async Task<Result<CourseResponse?, ValidationFailed>> Update(UpdateCourseRequest request, Guid id)
    {
        var course = request.MapToCourse(id);
        var validationResult = await _validator.ValidateAsync(course);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        _dbContext.Courses.Update(course);
        var numEntriesUpdated = await _dbContext.SaveChangesAsync();

        return numEntriesUpdated > 0 ? course.MapToResponse() : default;
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Courses.Where(x => x.Id == id).ExecuteDeleteAsync();
        return result > 0;
    }
}
