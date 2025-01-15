using Api.Students.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Movies.Api.Contracts.Responses;

namespace Api.Students;

public interface IStudentService
{
    Task<IEnumerable<StudentResponse>?> GetAllByCourseId(Guid courseId);
    Task<bool> AddToCourse(Guid courseId, AddStudentsRequest request);
    Task<Result<bool, ValidationResponse>> RemoveFromCourse(Guid courseId, Guid studentId);
}

public class StudentService : IStudentService
{
    private readonly AppDbContext _dbContext;

    public StudentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<StudentResponse>?> GetAllByCourseId(Guid courseId)
    {
        var course = await _dbContext
            .Courses
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        return course?.Students?.MapToStudentResponse();
    }

    public async Task<bool> AddToCourse(Guid courseId, AddStudentsRequest request)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return false;
        }

        var registeredUsers = await _dbContext.Users
            .Where(u => request.Emails.Contains(u.Email))
            .ToListAsync();
        var registeredEmails = registeredUsers.Select(u => u.Email);

        var unregisteredUsers = request.Emails
            .Except(registeredEmails)
            .Select(email => new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = "",
                Role = Role.Student,
            })
            .ToList();
        await _dbContext.Users.AddRangeAsync(unregisteredUsers);

        course.Students!.AddRange(registeredUsers);
        course.Students!.AddRange(unregisteredUsers);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<Result<bool, ValidationResponse>> RemoveFromCourse(Guid courseId, Guid studentId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return false;
        }

        var user = course.Students!.FirstOrDefault(t => t.Id == studentId);
        if (user is null)
        {
            return false;
        }

        course.Students!.Remove(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
