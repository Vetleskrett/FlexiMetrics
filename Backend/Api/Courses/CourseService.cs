using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database;
using Api.Courses.Contracts;
using Api.Teachers.Contracts;
using Api.Teams;
using Api.Teachers;

namespace Api.Courses;

public interface ICourseService
{
    Task<IEnumerable<CourseResponse>> GetAll();
    Task<IEnumerable<CourseResponse>> GetAllByTeacher(Guid teacherId);
    Task<IEnumerable<CourseResponse>> GetAllByStudent(Guid studentId);
    Task<CourseResponse?> GetById(Guid id);
    Task<TeacherCourseResponse?> GetByTeacher(Guid teacherId, Guid id);
    Task<StudentCourseResponse?> GetByStudent(Guid studentId, Guid id);
    Task<Result<CourseResponse, ValidationResponse>> Create(CreateCourseRequest request);
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
        var courses = await _dbContext.Courses.AsNoTracking().ToListAsync();
        return courses.MapToResponse();
    }

    public async Task<IEnumerable<CourseResponse>> GetAllByTeacher(Guid teacherId)
    {
        var courses = await _dbContext.CourseTeachers
            .AsNoTracking()
            .Where(x => x.TeacherId == teacherId)
            .Include(x => x.Course)
            .Select(x => x.Course)
            .ToListAsync();

        return courses!.MapToResponse();
    }

    public async Task<IEnumerable<CourseResponse>> GetAllByStudent(Guid studentId)
    {
        var courses = await _dbContext.CourseStudents
            .Include(x => x.Course)
            .Where(x => x.StudentId == studentId)
            .Select(x => x.Course!)
            .ToListAsync();

        return courses.MapToResponse();
    }

    public async Task<CourseResponse?> GetById(Guid id)
    {
        var course = await _dbContext.Courses.FindAsync(id);
        return course?.MapToResponse();
    }

    public async Task<TeacherCourseResponse?> GetByTeacher(Guid teacherId, Guid id)
    {
        var course = await _dbContext.Courses
            .Where(course => course.Id == id)
            .Select(course => new TeacherCourseResponse
            {
                Id = course.Id,
                Code = course.Code,
                Name = course.Name,
                Year = course.Year,
                Semester = course.Semester,
                NumStudents = _dbContext.CourseStudents
                    .Where(ct => ct.CourseId == course.Id)
                    .Count(),
                NumTeams = course.Teams != null ? course.Teams.Count : 0,
                Teachers = _dbContext.CourseTeachers
                    .Where(ct => ct.CourseId == course.Id)
                    .Select(ct => new TeacherResponse
                    {
                        Id = ct.TeacherId,
                        Email = ct.Teacher!.Email,
                        Name = ct.Teacher.Name
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        return course;
    }

    public async Task<StudentCourseResponse?> GetByStudent(Guid studentId, Guid id)
    {
        var course = await _dbContext.Courses.FindAsync(id);
        if (course is null)
        {
            return default;
        }

        var team = await _dbContext.Teams
            .Include(t => t.Students)
            .Where(t => t.CourseId == id && t.Students.Any(s => s.Id == studentId))
            .FirstOrDefaultAsync();

        var courseTeachers = await _dbContext.CourseTeachers
                .Include(ct => ct.Teacher)
                .Where(ct => ct.CourseId == course.Id)
                .ToListAsync();

        return new StudentCourseResponse
        {
            Id = course.Id,
            Code = course.Code,
            Name = course.Name,
            Year = course.Year,
            Semester = course.Semester,
            Team = team?.MapToResponse(),
            Teachers = courseTeachers.Select(ct => ct.Teacher)!.MapToTeacherResponse().ToList()
        };
    }

    public async Task<Result<CourseResponse, ValidationResponse>> Create(CreateCourseRequest request)
    {
        var course = request.MapToCourse();
        var validationResult = await _validator.ValidateAsync(course);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Courses.Add(course);
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
