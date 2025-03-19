using Api.Teachers.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Teachers;

public interface ITeacherService
{
    Task<Result<IEnumerable<TeacherResponse>>> GetAllByCourse(Guid courseId);
    Task<Result<IEnumerable<TeacherResponse>>> AddToCourse(Guid courseId, AddTeacherRequest request);
    Task<Result> RemoveFromCourse(Guid courseId, Guid teacherId);
}

public class TeacherService : ITeacherService
{
    private readonly AppDbContext _dbContext;

    public TeacherService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<TeacherResponse>>> GetAllByCourse(Guid courseId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.CourseTeachers!)
            .ThenInclude(ct => ct.Teacher)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return Result<IEnumerable<TeacherResponse>>.NotFound();
        }

        var teachers = course.CourseTeachers!
            .Select(ct => ct.Teacher!)
            .OrderBy(t => t.Email);

        return teachers.MapToTeacherResponse();
    }

    public async Task<Result<IEnumerable<TeacherResponse>>> AddToCourse(Guid courseId, AddTeacherRequest request)
    {
        var course = await _dbContext.Courses
            .Include(c => c.CourseTeachers!)
            .ThenInclude(ct => ct.Teacher)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return Result<IEnumerable<TeacherResponse>>.NotFound();
        }

        var teacher = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (teacher is null)
        {
            return Result<IEnumerable<TeacherResponse>>.NotFound();
        }

        var alreadyInCourse = course.CourseTeachers!.Any(ct =>
            ct.CourseId == courseId &&
            ct.TeacherId == teacher.Id
        );

        if (alreadyInCourse)
        {
            return new ValidationError("Teacher already in course").MapToResponse();
        }

        course.CourseTeachers!.Add(new CourseTeacher
        {
            CourseId = courseId,
            TeacherId = teacher.Id
        });
        await _dbContext.SaveChangesAsync();

        var teachers = course.CourseTeachers!
            .Select(ct => ct.Teacher!)
            .OrderBy(t => t.Email);

        return teachers.MapToTeacherResponse();
    }

    public async Task<Result> RemoveFromCourse(Guid courseId, Guid teacherId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.CourseTeachers)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return Result.NotFound();
        }

        var teacher = await _dbContext.Users.FindAsync(teacherId);

        if (teacher is null)
        {
            return Result.NotFound();
        }

        if (!course.CourseTeachers!.Any(ct => ct.TeacherId == teacherId))
        {
            return new ValidationError("Teacher is not a teacher in the course").MapToResponse();
        }

        if (course.CourseTeachers!.Count <= 1)
        {
            return new ValidationError("Course must have at least one teacher").MapToResponse();
        }

        course.CourseTeachers.RemoveAll(ct => ct.TeacherId == teacherId);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
