using Api.Progress.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Progress;

public interface IProgressService
{
    Task<Result<IEnumerable<ProgressResponse>>> GetCourseStudentsProgress(Guid courseId);
    Task<Result<IEnumerable<ProgressResponse>>> GetCourseTeamsProgress(Guid courseId);
    Task<Result<IEnumerable<AssignmentProgressResponse>>> GetCourseStudentProgress(Guid courseId, Guid studentId);
    Task<Result<IEnumerable<AssignmentProgressResponse>>> GetCourseTeamProgress(Guid courseId, Guid teamId);
}

public class ProgressService : IProgressService
{
    private readonly AppDbContext _dbContext;

    public ProgressService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<ProgressResponse>>> GetCourseStudentsProgress(Guid courseId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.CourseStudents!)
            .ThenInclude(cs => cs.Student)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return Result<IEnumerable<ProgressResponse>>.NotFound();
        }

        var assignments = await _dbContext.Assignments
            .Where(a => a.CourseId == courseId)
            .ToListAsync();

        var deliveries = await _dbContext.Deliveries
            .Include(d => d.Team!)
            .ThenInclude(t => t.Students)
            .Where(d => d.Assignment!.CourseId == courseId)
            .ToListAsync();

        var progress = course.CourseStudents!
            .Select(cs => cs.Student!)
            .Select(student =>
                new ProgressResponse
                {
                    Id = student.Id,
                    AssignmentsProgress = assignments.Select(assignment =>
                        new AssignmentProgressResponse
                        {
                            Id = assignment.Id,
                            IsDelivered = deliveries
                                .Where(d => d.AssignmentId == assignment.Id)
                                .Any(d => d.StudentId == student.Id || d.Team is not null && d.Team.Students.Any(s => s.Id == student.Id))
                        }
                    )
                    .ToList()
                }
            )
            .ToList();

        return progress;
    }

    public async Task<Result<IEnumerable<ProgressResponse>>> GetCourseTeamsProgress(Guid courseId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Teams)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return Result<IEnumerable<ProgressResponse>>.NotFound();
        }

        var assignments = await _dbContext.Assignments
            .Where(a => a.CourseId == courseId && a.CollaborationType == CollaborationType.Teams)
            .ToListAsync();

        var deliveries = await _dbContext.Deliveries
            .Where(d => d.Assignment!.CourseId == courseId)
            .ToListAsync();

        var progress = course.Teams!
            .Select(team =>
                new ProgressResponse
                {
                    Id = team.Id,
                    AssignmentsProgress = assignments.Select(assignment =>
                        new AssignmentProgressResponse
                        {
                            Id = assignment.Id,
                            IsDelivered = deliveries.Any(d => d.AssignmentId == assignment.Id && d.TeamId == team.Id)
                        }
                    )
                    .ToList()
                }
            )
            .ToList();

        return progress;
    }

    public async Task<Result<IEnumerable<AssignmentProgressResponse>>> GetCourseStudentProgress(Guid courseId, Guid studentId)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);
        if (course is null)
        {
            return Result<IEnumerable<AssignmentProgressResponse>>.NotFound();
        }

        var student = await _dbContext.Users.FindAsync(studentId);
        if (student is null)
        {
            return Result<IEnumerable<AssignmentProgressResponse>>.NotFound();
        }

        var courseStudent = await _dbContext.CourseStudents
            .FirstOrDefaultAsync(cs => cs.StudentId == studentId && cs.CourseId == courseId);

        if (courseStudent is null)
        {
            return new ValidationError("Student is not enrolled in the course").MapToResponse();
        }

        var assignments = await _dbContext.Assignments
            .Where(a => a.CourseId == courseId)
            .ToListAsync();

        var deliveries = await _dbContext.Deliveries
            .Where(d => d.Assignment!.CourseId == courseId)
            .Where(d => d.StudentId == studentId || d.Team != null && d.Team.Students.Any(s => s.Id == studentId))
            .ToListAsync();

        return assignments.Select(assignment =>
            new AssignmentProgressResponse
            {
                Id = assignment.Id,
                IsDelivered = deliveries.Any(d => d.AssignmentId == assignment.Id),
            }
        )
        .ToList();
    }

    public async Task<Result<IEnumerable<AssignmentProgressResponse>>> GetCourseTeamProgress(Guid courseId, Guid teamId)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);
        if (course is null)
        {
            return Result<IEnumerable<AssignmentProgressResponse>>.NotFound();
        }

        var team = await _dbContext.Teams.FindAsync(teamId);
        if (team is null)
        {
            return Result<IEnumerable<AssignmentProgressResponse>>.NotFound();
        }

        if (team.CourseId != courseId)
        {
            return new ValidationError("This team is not in the course").MapToResponse();
        }

        var assignments = await _dbContext.Assignments
            .Where(a => a.CourseId == courseId && a.CollaborationType == CollaborationType.Teams)
            .ToListAsync();

        var deliveries = await _dbContext.Deliveries
            .Where(d => d.Assignment!.CourseId == courseId && d.TeamId == teamId)
            .ToListAsync();

        return assignments.Select(assignment =>
            new AssignmentProgressResponse
            {
                Id = assignment.Id,
                IsDelivered = deliveries.Any(d => d.AssignmentId == assignment.Id),
            }
        )
        .ToList();
    }
}
