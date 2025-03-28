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
            .Where(a => a.Assignment!.CourseId == courseId)
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
            .Where(a => a.Assignment!.CourseId == courseId)
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
}
