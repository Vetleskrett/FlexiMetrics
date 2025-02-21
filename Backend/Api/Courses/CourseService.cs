using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database;
using Api.Courses.Contracts;
using Api.Teams;
using FileStorage;

namespace Api.Courses;

public interface ICourseService
{
    Task<Result<IEnumerable<CourseResponse>>> GetAll();
    Task<Result<IEnumerable<CourseResponse>>> GetAllByTeacher(Guid teacherId);
    Task<Result<IEnumerable<CourseResponse>>> GetAllByStudent(Guid studentId);
    Task<Result<CourseResponse>> GetById(Guid id);
    Task<Result<CourseResponse>> Create(CreateCourseRequest request);
    Task<Result<CourseResponse>> Update(UpdateCourseRequest request, Guid id);
    Task<Result> DeleteById(Guid id);
}

public class CourseService : ICourseService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Course> _validator;
    private readonly IFileStorage _fileStorage;

    public CourseService(AppDbContext dbContext, IValidator<Course> validator, IFileStorage fileStorage)
    {
        _dbContext = dbContext;
        _validator = validator;
        _fileStorage = fileStorage;
    }

    public async Task<Result<IEnumerable<CourseResponse>>> GetAll()
    {
        var courses = await _dbContext.Courses
            .AsNoTracking()
            .OrderByDescending(c => c.Year)
            .ThenByDescending(c => c.Semester)
            .ThenBy(c => c.Code)
            .ToListAsync();
        return courses.MapToResponse();
    }

    public async Task<Result<IEnumerable<CourseResponse>>> GetAllByTeacher(Guid teacherId)
    {
        var teacher = await _dbContext.Users.FindAsync(teacherId);
        if (teacher is null)
        {
            return Result<IEnumerable<CourseResponse>>.NotFound();
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

    public async Task<Result<IEnumerable<CourseResponse>>> GetAllByStudent(Guid studentId)
    {
        var student = await _dbContext.Users.FindAsync(studentId);
        if (student is null)
        {
            return Result<IEnumerable<CourseResponse>>.NotFound();
        }

        var courses = await _dbContext.CourseStudents
            .AsNoTracking()
            .Include(x => x.Course)
            .Where(x => x.StudentId == studentId)
            .Select(x => x.Course!)
            .OrderByDescending(c => c.Year)
            .ThenByDescending(c => c.Semester)
            .ThenBy(c => c.Code)
            .ToListAsync();

        return courses.MapToResponse();
    }

    public async Task<Result<CourseResponse>> GetById(Guid id)
    {
        var course = await _dbContext.Courses.FindAsync(id);
        if (course is null)
        {
            return Result<CourseResponse>.NotFound();
        }
        return course.MapToResponse();
    }

    public async Task<Result<CourseResponse>> Create(CreateCourseRequest request)
    {
        var teacher = await _dbContext.Users.FindAsync(request.TeacherId);
        if (teacher is null)
        {
            return Result<CourseResponse>.NotFound();
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

    public async Task<Result<CourseResponse>> Update(UpdateCourseRequest request, Guid id)
    {
        var course = await _dbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (course is null)
        {
            return Result<CourseResponse>.NotFound();
        }

        course = request.MapToCourse(id);

        var validationResult = await _validator.ValidateAsync(course);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Courses.Update(course);
        await _dbContext.SaveChangesAsync();

        return course.MapToResponse();
    }

    public async Task<Result> DeleteById(Guid id)
    {
        _fileStorage.DeleteCourse(id);
        var deleted = await _dbContext.Courses.Where(x => x.Id == id).ExecuteDeleteAsync();
        return deleted > 0 ? Result.Success() : Result.NotFound();
    }
}
