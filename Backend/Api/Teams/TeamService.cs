using Api.Teams.Contracts;
using Database;
using Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Teams;

public interface ITeamService
{
    Task<IEnumerable<TeamResponse>> GetAll();
    Task<IEnumerable<TeamResponse>> GetAllByCourseId(Guid courseId);
    Task<TeamResponse?> GetById(Guid id);
    Task<IEnumerable<TeamResponse>> Create(CreateTeamsRequest request);
    Task<bool> BulkAddToTeam(BulkAddStudentToTeamsRequest request);
    Task<bool> AddToTeam(Guid teamId, AddStudentToTeamRequest request);
    Task<bool> RemoveFromTeam(Guid teamId, Guid studentId);
    Task<bool> DeleteById(Guid id);
}

public class TeamService : ITeamService
{
    protected readonly AppDbContext _dbContext;
    protected readonly IValidator<Team> _validator;

    public TeamService(AppDbContext dbContext, IValidator<Team> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<IEnumerable<TeamResponse>> GetAll()
    {
        var teams = await _dbContext.Teams
            .Include(t => t.Students)
            .AsNoTracking()
            .ToListAsync();
        return teams.MapToResponse();
    }

    public async Task<IEnumerable<TeamResponse>> GetAllByCourseId(Guid courseId)
    {
        var teams = await _dbContext.Teams
            .Where(t => t.CourseId == courseId)
            .Include(t => t.Students)
            .AsNoTracking()
            .ToListAsync();

        return teams.MapToResponse();
    }

    public async Task<TeamResponse?> GetById(Guid id)
    {
        var team = await _dbContext.Teams
            .Include(t => t.Students)
            .FirstOrDefaultAsync(t => t.Id == id);
        return team?.MapToResponse();
    }

    public async Task<IEnumerable<TeamResponse>> Create(CreateTeamsRequest request)
    {
        var existingTeamNrs = await _dbContext.Teams
            .Where(t => t.CourseId == request.CourseId)
            .Select(t => t.TeamNr)
            .ToListAsync();

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

        return newteams.MapToResponse();
    }

    public async Task<bool> BulkAddToTeam(BulkAddStudentToTeamsRequest request)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Students)
            .Include(c => c.Teams)!
            .ThenInclude(t => t.Students)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId);

        if (course is null)
        {
            return false;
        }

        var allEmails = request.Teams
            .Select(t => t.Emails)
            .SelectMany(x => x)
            .ToList();

        var existingUsers = await _dbContext.Users
            .Where(u => allEmails.Contains(u.Email))
            .ToListAsync();

        var newUsers = allEmails
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

        var emailToUser = existingUsers
            .Union(newUsers)
            .ToDictionary(u => u.Email);

        foreach (var teamRequest in request.Teams)
        {
            var team = course.Teams!.FirstOrDefault(t => t.TeamNr == teamRequest.TeamNr);
            if (team is null)
            {
                team = new Team
                {
                    Id = Guid.NewGuid(),
                    CourseId = request.CourseId,
                    Course = course,
                    TeamNr = teamRequest.TeamNr,
                    Students = [],
                };
                _dbContext.Teams.Add(team);
            }

            var teamMembers = teamRequest.Emails
                .Select(email => emailToUser[email])
                .ToList();

            course.Students!.AddRange(teamMembers);
            team.Students.AddRange(teamMembers);
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddToTeam(Guid teamId, AddStudentToTeamRequest request)
    {
        var team = await _dbContext.Teams
            .Include(t => t.Students)
            .Include(t => t.Course)
            .ThenInclude(c => c!.Students)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team is null)
        {
            return false;
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(t => t.Email == request.Email);
        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = "",
                Role = Role.Student,
            };
            _dbContext.Users.Add(user);
        }

        team.Course!.Students!.Add(user);
        team.Students.Add(user);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFromTeam(Guid teamId, Guid studentId)
    {
        var team = await _dbContext.Teams
            .Include(t => t.Students)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team is null)
        {
            return false;
        }

        var removed = team.Students.RemoveAll(t => t.Id == studentId);
        if (removed == 0)
        {
            return false;
        }

        await _dbContext.SaveChangesAsync();
        return true;

    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Teams
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
        return result > 0;
    }
}