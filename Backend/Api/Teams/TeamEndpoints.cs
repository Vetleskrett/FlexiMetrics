using Api.Teams.Contracts;

namespace Api.Teams;

public static class TeamEndpoints
{
    public static void MapTeamEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Teams");

        group.MapGet("teams", async (ITeamService teamService) =>
        {
            var teams = await teamService.GetAll();
            return Results.Ok(teams);
        })
        .Produces<IEnumerable<TeamResponse>>()
        .WithName("GetAllTeams")
        .WithSummary("Get all teams");

        group.MapGet("courses/{courseId:guid}/teams", async (ITeamService teamService, Guid courseId) =>
        {
            var teams = await teamService.GetAllByCourse(courseId);
            return Results.Ok(teams);
        })
        .Produces<IEnumerable<TeamResponse>>()
        .WithName("GetAllTeamsByCourse")
        .WithSummary("Get all teams by course id");

        group.MapGet("teams/{id:guid}", async (ITeamService teamService, Guid id) =>
        {
            var team = await teamService.GetById(id);
            return team is not null ? Results.Ok(team) : Results.NotFound();
        })
        .Produces<TeamResponse>()
        .WithName("GetTeam")
        .WithSummary("Get team by id");

        group.MapPost("teams", async (ITeamService teamService, CreateTeamsRequest request) =>
        {
            var teams = await teamService.Create(request);
            return Results.CreatedAtRoute
            (
                "GetAllTeamsByCourse",
                new { courseId = request.CourseId },
                teams
            );
        })
        .Produces<IEnumerable<TeamResponse>>()
        .WithName("CreateTeams")
        .WithSummary("Create new teams");

        group.MapPost("teams/bulk", async (ITeamService teamService, BulkAddStudentToTeamsRequest request) =>
        {
            var added = await teamService.BulkAddToTeam(request);
            return added ? Results.Ok() : Results.NotFound();
        })
        .WithName("BulkAddStudentToTeams")
        .WithSummary("Bulk add student to teams");

        group.MapPost("teams/{teamId:guid}/students", async (ITeamService teamService, Guid teamId, AddStudentToTeamRequest request) =>
        {
            var added = await teamService.AddToTeam(teamId, request);
            return added ? Results.Ok() : Results.NotFound();
        })
        .WithName("AddStudentToTeam")
        .WithSummary("Add student to team");

        group.MapPost("teams/{teamId:guid}/students/{studentId:guid}", async (ITeamService teamService, Guid teamId, Guid studentId) =>
        {
            var added = await teamService.RemoveFromTeam(teamId, studentId);
            return added ? Results.Ok() : Results.NotFound();
        })
        .WithName("RemoveStudentFromTeam")
        .WithSummary("Remove student from team");

        group.MapDelete("teams/{teamId:guid}", async (ITeamService teamService, Guid teamId) =>
        {
            var deleted = await teamService.DeleteById(teamId);
            return deleted ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteTeam")
        .WithSummary("Delete team by id");
    }
}