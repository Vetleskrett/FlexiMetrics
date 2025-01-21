using Api.Students.Contracts;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Students;

public interface IStudentService
{
    Task<IEnumerable<StudentResponse>> GetAllByCourse(Guid courseId);
    Task<bool> AddToCourse(Guid courseId, AddStudentsToCourseRequest request);
    Task<bool> RemoveFromCourse(Guid courseId, Guid studentId);
}

public class StudentService : IStudentService
{
    private readonly AppDbContext _dbContext;

    public StudentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<StudentResponse>> GetAllByCourse(Guid courseId)
    {
        var students = await _dbContext.CourseStudents
            .Include(c => c.Student)
            .Where(x => x.CourseId == courseId)
            .Select(x => x.Student!)
            .ToListAsync();

        return students.MapToStudentResponse();
    }

    public async Task<bool> AddToCourse(Guid courseId, AddStudentsToCourseRequest request)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);
        if (course is null)
        {
            return false;
        }

        var existingUsers = await _dbContext.Users
            .Where(u => request.Emails.Contains(u.Email))
            .ToListAsync();
        var existingUsersEmails = existingUsers.Select(u => u.Email);
        var existingUsersIds = existingUsers.Select(u => u.Id);

        var newUsers = request.Emails
            .Except(existingUsersEmails)
            .Select(email => new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = "",
                Role = Role.Student,
            })
            .ToList();
        _dbContext.Users.AddRange(newUsers);

        var alreadyInCourse = await _dbContext.CourseStudents
            .Where(x => existingUsersIds.Contains(x.StudentId))
            .Select(x => x.StudentId)
            .ToListAsync();

        var courseStudents = existingUsersIds
            .Except(alreadyInCourse)
            .Union(newUsers.Select(x => x.Id))
            .Select(studentId => new CourseStudent
            {
                CourseId = courseId,
                StudentId = studentId,
            });

        _dbContext.CourseStudents.AddRange(courseStudents);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFromCourse(Guid courseId, Guid studentId)
    {
        var removed = await _dbContext.CourseStudents
            .Where(x => x.CourseId == courseId && x.StudentId == studentId)
            .ExecuteDeleteAsync();

        return removed > 0;
    }
}
