using Api.Students.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using FileStorage;
using Microsoft.EntityFrameworkCore;

namespace Api.Students;

public interface IStudentService
{
    Task<Result<StudentResponse>> GetById(Guid studentId);
    Task<Result<IEnumerable<CourseStudentResponse>>> GetAllByCourse(Guid courseId);
    Task<Result<IEnumerable<CourseStudentResponse>>> AddToCourse(Guid courseId, AddStudentsToCourseRequest request);
    Task<Result> RemoveFromCourse(Guid courseId, Guid studentId);
}

public class StudentService : IStudentService
{
    private readonly AppDbContext _dbContext;
    private readonly IFileStorage _fileStorage;

    public StudentService(AppDbContext dbContext, IFileStorage fileStorage)
    {
        _dbContext = dbContext;
        _fileStorage = fileStorage;
    }

    public async Task<Result<StudentResponse>> GetById(Guid studentId)
    {
        var student = await _dbContext.Users.FindAsync(studentId);

        if (student is null)
        {
            return Result<StudentResponse>.NotFound();
        }

        return student.MapToStudentResponse();
    }

    public async Task<Result<IEnumerable<CourseStudentResponse>>> GetAllByCourse(Guid courseId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Teams!)
            .ThenInclude(c => c.Students)
            .Include(c => c.CourseStudents!)
            .ThenInclude(cs => cs.Student)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return Result<IEnumerable<CourseStudentResponse>>.NotFound();
        }

        var students = course.CourseStudents!
            .Select(cs => cs.Student!)
            .OrderBy(s => s.Email)
            .Select(s =>
                new CourseStudentResponse
                {
                    Id = s.Id,
                    Email = s.Email,
                    Name = s.Name,
                    TeamNr = course.Teams!.FirstOrDefault(t => t.Students.Any(ts => ts.Id == s.Id))?.TeamNr,
                }
            )
            .ToList();

        return students;
    }

    public async Task<Result<IEnumerable<CourseStudentResponse>>> AddToCourse(Guid courseId, AddStudentsToCourseRequest request)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Teams!)
            .ThenInclude(c => c.Students)
            .Include(c => c.CourseStudents!)
            .ThenInclude(cs => cs.Student)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return Result<IEnumerable<CourseStudentResponse>>.NotFound();
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
            .Select(cs => cs.Student!)
            .OrderBy(s => s.Email)
            .Select(s =>
                new CourseStudentResponse
                {
                    Id = s.Id,
                    Email = s.Email,
                    Name = s.Name,
                    TeamNr = course.Teams!.FirstOrDefault(t => t.Students.Any(ts => ts.Id == s.Id))?.TeamNr,
                }
            )
            .ToList();

        return students;
    }

    public async Task<Result> RemoveFromCourse(Guid courseId, Guid studentId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.CourseStudents)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return Result.NotFound();
        }

        var student = await _dbContext.Users.FindAsync(studentId);

        if (student is null)
        {
            return Result.NotFound();
        }

        if (!course.CourseStudents!.Any(ct => ct.StudentId == studentId))
        {
            return new ValidationError("Student is not enrolled in the course").MapToResponse();
        }

        var deliveries = await _dbContext.Deliveries
            .Where(d => d.StudentId == studentId)
            .Where(d => d.Assignment!.CourseId == courseId)
            .ToListAsync();

        foreach (var delivery in deliveries)
        {
            _fileStorage.DeleteDelivery(courseId, delivery.AssignmentId, delivery.Id);
        }

        course.CourseStudents!.RemoveAll(ct => ct.StudentId == studentId);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
