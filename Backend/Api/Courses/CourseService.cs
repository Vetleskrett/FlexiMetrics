using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database;
using Api.Courses.Contracts;
using Api.Teams;

namespace Api.Courses;

public interface ICourseService
{
    Task<IEnumerable<CourseResponse>> GetAll();
    Task<IEnumerable<CourseResponse>?> GetAllByTeacher(Guid teacherId);
    Task<IEnumerable<CourseResponse>?> GetAllByStudent(Guid studentId);
    Task<CourseResponse?> GetById(Guid id);
    Task<Result<CourseResponse?, ValidationResponse>> Create(CreateCourseRequest request);
    Task<Result<CourseResponse?, ValidationResponse>> Update(UpdateCourseRequest request, Guid id);
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
        var courses = await _dbContext.Courses
            .AsNoTracking()
            .OrderByDescending(c => c.Year)
            .ThenByDescending(c => c.Semester)
            .ThenBy(c => c.Code)
            .ToListAsync();
        return courses.MapToResponse();
    }

    public async Task<IEnumerable<CourseResponse>?> GetAllByTeacher(Guid teacherId)
    {
        var teacher = await _dbContext.Users.FindAsync(teacherId);
        if (teacher is null)
        {
            return default;
        }

        var courses = await _dbContext.CourseTeachers
            .AsNoTracking()
            .Where(x => x.TeacherId == teacherId)
            .Include(x => x.Course)
            .Select(x => x.Course!)
            .OrderByDescending(c => c.Year)
            .ThenByDescending(c => c.Semester)
            .ThenBy(c => c.Code)
            .ToListAsync();

        return courses!.MapToResponse();
    }

    public async Task<IEnumerable<CourseResponse>?> GetAllByStudent(Guid studentId)
    {
        var student = await _dbContext.Users.FindAsync(studentId);
        if (student is null)
        {
            return default;
        }

        var courses = await _dbContext.CourseStudents
            .Include(x => x.Course)
            .Where(x => x.StudentId == studentId)
            .Select(x => x.Course!)
            .OrderByDescending(c => c.Year)
            .ThenByDescending(c => c.Semester)
            .ThenBy(c => c.Code)
            .ToListAsync();

        return courses.MapToResponse();
    }

    public async Task<CourseResponse?> GetById(Guid id)
    {
        var course = await _dbContext.Courses.FindAsync(id);
        return course?.MapToResponse();
    }

    public async Task<Result<CourseResponse?, ValidationResponse>> Create(CreateCourseRequest request)
    {
        var teacher = await _dbContext.Users.FindAsync(request.TeacherId);
        if (teacher is null)
        {
            return default;
        }

        var course = request.MapToCourse();
        var validationResult = await _validator.ValidateAsync(course);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Courses.Add(course);
        _dbContext.CourseTeachers.Add(new CourseTeacher
        {
            CourseId = course.Id,
            TeacherId = teacher.Id,
        });
        await _dbContext.SaveChangesAsync();

        return course.MapToResponse();
    }

    public async Task<Result<CourseResponse?, ValidationResponse>> Update(UpdateCourseRequest request, Guid id)
    {
        var course = await _dbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (course is null)
        {
            return default;
        }

        course = request.MapToCourse(id);

        var validationResult = await _validator.ValidateAsync(course);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
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
