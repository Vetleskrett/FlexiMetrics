﻿using Api.Teams.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using FileStorage;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Teams;

public interface ITeamService
{
    Task<Result<IEnumerable<TeamResponse>>> GetAll();
    Task<Result<IEnumerable<TeamResponse>>> GetAllByCourse(Guid courseId);
    Task<Result<TeamResponse>> GetById(Guid id);
    Task<Result<TeamResponse>> GetByStudentCourse(Guid studentId, Guid courseId);
    Task<Result<IEnumerable<TeamResponse>>> Create(CreateTeamsRequest request);
    Task<Result<IEnumerable<TeamResponse>>> BulkAddToTeams(BulkAddStudentsToTeamsRequest request);
    Task<Result<TeamResponse>> AddToTeam(Guid teamId, AddStudentToTeamRequest request);
    Task<Result<TeamResponse>> AddToTeam(Guid teamId, Guid studentId);
    Task<Result<TeamResponse>> RemoveFromTeam(Guid teamId, Guid studentId);
    Task<Result> DeleteById(Guid id);
}

public class TeamService : ITeamService
{
    private readonly AppDbContext _dbContext;
    private readonly IFileStorage _fileStorage;
    public TeamService(AppDbContext dbContext, IFileStorage fileStorage)
    {
        _dbContext = dbContext;
        _fileStorage = fileStorage;
    }

    public async Task<Result<IEnumerable<TeamResponse>>> GetAll()
    {
        var teams = await _dbContext.Teams
            .Include(t => t.Students.OrderBy(s => s.Email))
            .OrderBy(t => t.Course!.Code)
            .ThenBy(t => t.TeamNr)
            .AsNoTracking()
            .ToListAsync();
        return teams.MapToResponse();
    }

    public async Task<Result<IEnumerable<TeamResponse>>> GetAllByCourse(Guid courseId)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Teams!.OrderBy(t => t.TeamNr))
            .ThenInclude(t => t.Students.OrderBy(s => s.Email))
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null)
        {
            return Result<IEnumerable<TeamResponse>>.NotFound();
        }

        return course.Teams!.MapToResponse();
    }

    public async Task<Result<TeamResponse>> GetById(Guid id)
    {
        var team = await _dbContext.Teams
            .Include(t => t.Students.OrderBy(s => s.Email))
            .FirstOrDefaultAsync(t => t.Id == id);

        if (team is null)
        {
            return Result<TeamResponse>.NotFound();
        }

        return team.MapToResponse();
    }

    public async Task<Result<TeamResponse>> GetByStudentCourse(Guid studentId, Guid courseId)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);
        if (course is null)
        {
            return Result<TeamResponse>.NotFound();
        }

        var student = await _dbContext.Users.FindAsync(studentId);
        if (student is null)
        {
            return Result<TeamResponse>.NotFound();
        }

        var courseStudent = await _dbContext.CourseStudents
            .FirstOrDefaultAsync(cs => cs.CourseId == courseId && cs.StudentId == studentId);

        if (courseStudent is null)
        {
            return new ValidationError("Student is not enrolled in the course").MapToResponse();
        }

        var team = await _dbContext.Teams
            .Include(t => t.Students.OrderBy(s => s.Email))
            .Where(t => t.CourseId == courseId)
            .Where(t => t.Students.Any(s => s.Id == studentId))
            .FirstOrDefaultAsync();

        if (team is null)
        {
            return Result<TeamResponse>.NoContent();
        }

        return team.MapToResponse();
    }

    public async Task<Result<IEnumerable<TeamResponse>>> Create(CreateTeamsRequest request)
    {
        if (request.NumTeams <= 0)
        {
            return new ValidationError("Number of teams must be a positive integer")
            {
                PropertyName = nameof(request.NumTeams)
            }
            .MapToResponse();
        }

        var course = await _dbContext.Courses
            .Include(c => c.Teams!)
            .ThenInclude(t => t.Students)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId);

        if (course is null)
        {
            return Result<IEnumerable<TeamResponse>>.NotFound();
        }

        var existingTeamNrs = course.Teams!
            .Select(t => t.TeamNr)
            .ToList();

        var totalNumTeams = existingTeamNrs.Count + request.NumTeams;
        var newTeamNrs = Enumerable.Range(1, totalNumTeams)
            .Except(existingTeamNrs)
            .Take(request.NumTeams);

        var newteams = newTeamNrs.Select(teamNr => new Team
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            TeamNr = teamNr,
            Students = []
        }).ToList();

        _dbContext.Teams.AddRange(newteams);

        await _dbContext.SaveChangesAsync();

        return course.Teams!
            .OrderBy(t => t.TeamNr)
            .Select(t =>
            {
                t.Students = t.Students.OrderBy(s => s.Email).ToList();
                return t;
            })
            .MapToResponse()
            .ToList();
    }

    private async Task<List<User>> GetOrCreateStudents(List<string> emails)
    {
        var existingUsers = await _dbContext.Users
            .Where(u => emails.Contains(u.Email))
            .ToListAsync();

        var newUsers = emails
            .Except(existingUsers.Select(u => u.Email))
            .Select(email => new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = "",
                Role = Role.Student,
            })
            .ToList();

        _dbContext.Users.AddRange(newUsers);

        return existingUsers.Union(newUsers).ToList();
    }

    private void EnsureStudentsInCourse(Course course, List<User> students)
    {
        foreach (var student in students)
        {
            var inCourse = course.CourseStudents!.Any(cs => cs.StudentId == student.Id);
            if (!inCourse)
            {
                _dbContext.CourseStudents.Add(new CourseStudent
                {
                    CourseId = course.Id,
                    StudentId = student.Id,
                });
            }
        }
    }

    private void BulkAddToTeam(Course course, List<User> students, TeamRequest request)
    {
        var team = course.Teams!.FirstOrDefault(t => t.TeamNr == request.TeamNr);

        if (team is null)
        {
            team = new Team
            {
                Id = Guid.NewGuid(),
                CourseId = course.Id,
                TeamNr = request.TeamNr,
                Students = [],
            };
            course.Teams!.Add(team);
            _dbContext.Teams.Add(team);
        }

        foreach (var email in request.Emails)
        {

            if (!team.Students.Any(s => s.Email == email))
            {
                team.Students.Add(students.First(s => s.Email == email));
            }
        }
    }

    public async Task<Result<IEnumerable<TeamResponse>>> BulkAddToTeams(BulkAddStudentsToTeamsRequest request)
    {
        var course = await _dbContext.Courses
            .Include(c => c.CourseStudents!)
            .Include(c => c.Teams!)
            .ThenInclude(t => t.Students)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId);

        if (course is null)
        {
            return Result<IEnumerable<TeamResponse>>.NotFound();
        }

        var emails = request.Teams
            .Select(t => t.Emails)
            .SelectMany(x => x)
            .ToList();

        var students = await GetOrCreateStudents(emails);
        EnsureStudentsInCourse(course, students);

        foreach (var teamRequest in request.Teams)
        {
            BulkAddToTeam(course, students, teamRequest);
        }

        await _dbContext.SaveChangesAsync();

        return course.Teams!
            .OrderBy(t => t.TeamNr)
            .Select(t =>
            {
                t.Students = t.Students.OrderBy(s => s.Email).ToList();
                return t;
            })
            .MapToResponse();
    }

    public async Task<Result<TeamResponse>> AddToTeam(Guid teamId, AddStudentToTeamRequest request)
    {
        var team = await _dbContext.Teams
            .Include(t => t.Students)
            .Include(t => t.Course!)
            .ThenInclude(c => c.CourseStudents)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team is null)
        {
            return Result<TeamResponse>.NotFound();
        }

        var student = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (student is null)
        {
            student = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = "",
                Role = Role.Student,
            };
            _dbContext.Users.Add(student);
        }

        if (team.Students.Any(s => s.Id == student.Id))
        {
            return new ValidationError("Student already in team").MapToResponse();
        }

        team.Students.Add(student);

        if (!team.Course!.CourseStudents!.Any(cs => cs.StudentId == student.Id))
        {
            _dbContext.CourseStudents.Add(new CourseStudent
            {
                CourseId = team.CourseId,
                StudentId = student.Id,
            });
        }

        await _dbContext.SaveChangesAsync();

        return team.MapToResponse();
    }

    public async Task<Result<TeamResponse>> AddToTeam(Guid teamId, Guid studentId)
    {
        var team = await _dbContext.Teams
            .Include(t => t.Students)
            .Include(t => t.Course!)
            .ThenInclude(c => c.CourseStudents)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team is null)
        {
            return Result<TeamResponse>.NotFound();
        }

        var student = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == studentId);
        if (student is null)
        {
            return Result<TeamResponse>.NotFound();
        }

        if (team.Students.Any(s => s.Id == student.Id))
        {
            return new ValidationError("Student already in team").MapToResponse();
        }

        team.Students.Add(student);

        if (!team.Course!.CourseStudents!.Any(cs => cs.StudentId == student.Id))
        {
            return new ValidationError("Student not in course").MapToResponse();
        }

        await _dbContext.SaveChangesAsync();

        return team.MapToResponse();
    }

    public async Task<Result<TeamResponse>> RemoveFromTeam(Guid teamId, Guid studentId)
    {
        var team = await _dbContext.Teams
            .Include(t => t.Students)
            .FirstOrDefaultAsync(t => t.Id == teamId);
        if (team is null)
        {
            return Result<TeamResponse>.NotFound();
        }

        var student = await _dbContext.Users.FindAsync(studentId);
        if (student is null)
        {
            return Result<TeamResponse>.NotFound();
        }

        var inTeam = team.Students.Any(s => s.Id == studentId);
        if (!inTeam)
        {
            return new ValidationError("Student is not in the team").MapToResponse();
        }

        team.Students.RemoveAll(t => t.Id == studentId);
        await _dbContext.SaveChangesAsync();

        return team.MapToResponse();
    }

    public async Task<Result> DeleteById(Guid id)
    {
        var deliveries = await _dbContext.Deliveries
            .Include(d => d.Team)
            .Where(d => d.TeamId == id)
            .ToListAsync();

        foreach (var delivery in deliveries)
        {
            _fileStorage.DeleteDelivery(delivery.Team!.CourseId, delivery.AssignmentId, delivery.Id);
        }

        var deleted = await _dbContext.Teams.Where(x => x.Id == id).ExecuteDeleteAsync();
        return deleted > 0 ? Result.Success() : Result.NotFound();
    }
}