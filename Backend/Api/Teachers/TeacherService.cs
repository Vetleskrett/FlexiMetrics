using Api.Teachers.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Teachers;

public interface ITeacherService
{
    Task<IEnumerable<TeacherResponse>?> GetAllByCourse(Guid courseId);
    Task<bool> AddToCourse(Guid courseId, AddTeacherRequest request);
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
        var teachers = await _dbContext.CourseTeachers
            .AsNoTracking()
            .Include(x => x.Teacher)
            .Where(x => x.CourseId == courseId)
            .Select(x => x.Teacher)
            .ToListAsync();

        return teachers!.MapToTeacherResponse();
    }

    public async Task<bool> AddToCourse(Guid courseId, AddTeacherRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        var course = await _dbContext.Courses.FindAsync(courseId);

        if (user is null || course is null)
        {
            return false;
        }

        _dbContext.CourseTeachers.Add(new CourseTeacher
        {
            CourseId = courseId,
            TeacherId = user.Id
        });
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<Result<bool, ValidationResponse>> RemoveFromCourse(Guid courseId, Guid teacherId)
    {
        var numTeachers = await _dbContext.CourseTeachers
            .Where(x => x.CourseId == courseId)
            .CountAsync();

        if (numTeachers <= 1)
        {
            return new ValidationError
            {
                PropertyName = nameof(Teachers),
                Message = "Course must have at least one teacher."
            }.MapToResponse();
        }

        var numDeleted = await _dbContext.CourseTeachers
            .Where(x => x.CourseId == courseId && x.TeacherId == teacherId)
            .ExecuteDeleteAsync();

        return numDeleted > 0;
    }
}
