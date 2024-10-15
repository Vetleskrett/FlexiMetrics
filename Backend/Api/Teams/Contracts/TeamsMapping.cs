using Api.Courses.Contracts;
using Database.Models;

namespace Api.Teams.Contracts
{
    public static class TeamsMapping
    {
        public static Team MapToTeam(this CreateTeamRequest request)
        {
            return new Team
            {
                Id = Guid.NewGuid(),
                CourseId = request.CourseId,
                TeamId = request.TeamId,
            };
        }

        public static Team MapToTeam(this UpdateTeamRequest request, Guid id)
        {
            return new Team
            {
                Id = id,
                CourseId = request.CourseId,
                TeamId = request.TeamId,
            };
        }

        public static TeamResponse MapToResponse(this Team team)
        {
            return new TeamResponse
            {
                Id = team.Id,
                CourseId = team.CourseId,
                TeamId = team.TeamId,
            };
        }

        public static IEnumerable<TeamResponse> MapToResponse(this IEnumerable<Team> teams)
        {
            return teams.Select(team => team.MapToResponse());
        }
    }
}
