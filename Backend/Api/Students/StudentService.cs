using Api.Students.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Students;

public interface IStudentService
{
    Task<StudentResponse?> GetById(Guid studentId);
    Task<IEnumerable<StudentResponse>?> GetAllByCourse(Guid courseId);
    Task<IEnumerable<StudentResponse>?> AddToCourse(Guid courseId, AddStudentsToCourseRequest request);
    Task<Result<bool, ValidationResponse>> RemoveFromCourse(Guid courseId, Guid studentId);
}

public class StudentService : IStudentService
{
    private readonly AppDbContext _dbContext;

    public StudentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StudentResponse?> GetById(Guid studentId)
    {
        var student = await _dbContext.Users.FindAsync(studentId);
        return student?.MapToStudentResponse();
    }

    public async Task<IEnumerable<StudentResponse>?> GetAllByCourse(Guid courseId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.CourseStudents!)
            .ThenInclude(cs => cs.Student)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return default;
        }

        var students = course.CourseStudents!
            .Select(ct => ct.Student!)
            .OrderBy(t => t.Email);

        return students.MapToStudentResponse();
    }

    public async Task<IEnumerable<StudentResponse>?> AddToCourse(Guid courseId, AddStudentsToCourseRequest request)
    {
        var course = await _dbContext.Courses
            .Include(c => c.CourseStudents!)
            .ThenInclude(cs => cs.Student)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return default;
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

        var alreadyInCourse = course.CourseStudents!
            .Where(x => existingUsersIds.Contains(x.StudentId))
            .Select(x => x.StudentId)
            .ToList();

        var newCourseStudents = existingUsersIds
            .Except(alreadyInCourse)
            .Union(newUsers.Select(x => x.Id))
            .Select(studentId => new CourseStudent
            {
                CourseId = courseId,
                StudentId = studentId,
            });
        course.CourseStudents!.AddRange(newCourseStudents);

        await _dbContext.SaveChangesAsync();

        var students = course.CourseStudents!
            .Select(ct => ct.Student!)
            .OrderBy(t => t.Email);

        return students.MapToStudentResponse();
    }

    public async Task<Result<bool, ValidationResponse>> RemoveFromCourse(Guid courseId, Guid studentId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.CourseStudents)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        var student = await _dbContext.Users.FindAsync(studentId);

        if (course is null || student is null)
        {
            return false;
        }

        if (!course.CourseStudents!.Any(ct => ct.StudentId == studentId))
        {
            return new ValidationError("Student is not enrolled in the course").MapToResponse();
        }

        course.CourseStudents!.RemoveAll(ct => ct.StudentId == studentId);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
