using Api.Teachers.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Teachers;

public interface ITeacherService
{
    Task<IEnumerable<TeacherResponse>?> GetAllByCourse(Guid courseId);
    Task<Result<bool, ValidationResponse>> AddToCourse(Guid courseId, AddTeacherRequest request);
    Task<Result<bool, ValidationResponse>> RemoveFromCourse(Guid courseId, Guid teacherId);
}

public class TeacherService : ITeacherService
{
    private readonly AppDbContext _dbContext;

    public TeacherService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TeacherResponse>?> GetAllByCourse(Guid courseId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.CourseTeachers!)
            .ThenInclude(ct => ct.Teacher)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return default;
        }

        var teachers = course.CourseTeachers!
            .Select(ct => ct.Teacher!)
            .OrderBy(t => t.Email);

        return teachers.MapToTeacherResponse();
    }

    public async Task<Result<bool, ValidationResponse>> AddToCourse(Guid courseId, AddTeacherRequest request)
    {
        var teacher = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        var course = await _dbContext.Courses.FindAsync(courseId);

        if (teacher is null || course is null)
        {
            return false;
        }

        var alreadyInCourse = await _dbContext.CourseTeachers
            .AnyAsync(ct => ct.CourseId == courseId && ct.TeacherId == teacher.Id);

        if (alreadyInCourse)
        {
            return new ValidationError("Teacher already in course").MapToResponse();
        }

        _dbContext.CourseTeachers.Add(new CourseTeacher
        {
            CourseId = courseId,
            TeacherId = teacher.Id
        });
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<Result<bool, ValidationResponse>> RemoveFromCourse(Guid courseId, Guid teacherId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.CourseTeachers)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        var teacher = await _dbContext.Users.FindAsync(teacherId);

        if (course is null || teacher is null)
        {
            return false;
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

        return true;
    }
}
