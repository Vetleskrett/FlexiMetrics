using Api.Teachers.Contracts;
using Api.Validation;
using Database;
using Microsoft.EntityFrameworkCore;
using Movies.Api.Contracts.Responses;

namespace Api.Teachers;

public interface ITeacherService
{
    Task<IEnumerable<TeacherResponse>?> GetAllByCourseId(Guid courseId);
    Task<bool> AddToCourse(Guid courseId, AddStudentRequest request);
    Task<Result<bool, ValidationResponse>> RemoveFromCourse(Guid courseId, Guid teacherId);
}

public class TeacherService : ITeacherService
{
    private readonly AppDbContext _dbContext;

    public TeacherService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TeacherResponse>?> GetAllByCourseId(Guid courseId)
    {
        var course = await _dbContext
            .Courses
            .Include(c => c.Teachers)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        return course?.Teachers?.MapToTeacherResponse();
    }

    public async Task<bool> AddToCourse(Guid courseId, AddStudentRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        var course = await _dbContext.Courses
            .Include(c => c.Teachers)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (user is null || course is null)
        {
            return false;
        }

        course.Teachers!.Add(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Result<bool, ValidationResponse>> RemoveFromCourse(Guid courseId, Guid teacherId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Teachers)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return false;
        }

        if (course.Teachers!.Count == 1)
        {
            return new ValidationError
            {
                PropertyName = nameof(Teachers),
                Message = "Course must have at least one teacher."
            }.MapToResponse();
        }

        var removed = course.Teachers!.RemoveAll(t => t.Id == teacherId);
        if (removed == 0)
        {
            return false;
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }
}
