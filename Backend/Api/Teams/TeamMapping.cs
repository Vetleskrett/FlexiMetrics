using Api.Students;
using Api.Teams.Contracts;
using Database.Models;

namespace Api.Teams;

public static class TeamMapping
{
    public static IEnumerable<TeamResponse> MapToResponse(this IEnumerable<Team> teams)
    {
        return teams.Select(team => team.MapToResponse());
    }

    public static TeamResponse MapToResponse(this Team team)
    {
        return new TeamResponse
        {
            Id = team.Id,
            CourseId = team.CourseId,
            TeamNr = team.TeamNr,
            Students = team.Students.MapToStudentResponse().ToList(),
        };
    }
}
