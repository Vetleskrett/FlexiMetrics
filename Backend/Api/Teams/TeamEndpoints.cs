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
            return teams is not null ? Results.Ok(teams) : Results.NotFound();
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

        group.MapGet("students/{studentId:guid}/courses/{courseId:guid}/teams", async (ITeamService teamService, Guid studentId, Guid courseId) =>
        {
            var result = await teamService.GetByStudentCourse(studentId, courseId);
            return result.Match
            (
                team => team is not null ? Results.Ok(team) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<TeamResponse>()
        .WithName("GetTeamByStudentCourse")
        .WithSummary("Get team by student id and course id");

        group.MapPost("teams", async (ITeamService teamService, CreateTeamsRequest request) =>
        {
            var result = await teamService.Create(request);
            return result.Match
            (
                teams => teams is not null ? Results.CreatedAtRoute
                (
                    "GetAllTeamsByCourse",
                    new { courseId = request.CourseId },
                    teams
                ) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<IEnumerable<TeamResponse>>()
        .WithName("CreateTeams")
        .WithSummary("Create new teams");

        group.MapPost("teams/bulk", async (ITeamService teamService, BulkAddStudentsToTeamsRequest request) =>
        {
            var teams = await teamService.BulkAddToTeams(request);
            return teams is not null ? Results.Ok(teams) : Results.NotFound();
        })
        .Produces<IEnumerable<TeamResponse>>()
        .WithName("BulkAddStudentsToTeams")
        .WithSummary("Bulk add students to teams");

        group.MapPost("teams/{teamId:guid}/students", async (ITeamService teamService, Guid teamId, AddStudentToTeamRequest request) =>
        {
            var result = await teamService.AddToTeam(teamId, request);
            return result.Match
            (
                team => team is not null ? Results.Ok(team) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<TeamResponse>()
        .WithName("AddStudentEmailToTeam")
        .WithSummary("Add student to team");

        group.MapPost("teams/{teamId:guid}/students/{studentId:guid}", async (ITeamService teamService, Guid teamId, Guid studentId) =>
        {
            var result = await teamService.AddToTeam(teamId, studentId);
            return result.Match
            (
                team => team is not null ? Results.Ok(team) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<TeamResponse>()
        .WithName("AddStudentIdToTeam")
        .WithSummary("Add student to team");

        group.MapDelete("teams/{teamId:guid}/students/{studentId:guid}", async (ITeamService teamService, Guid teamId, Guid studentId) =>
        {
            var result = await teamService.RemoveFromTeam(teamId, studentId);
            return result.Match
            (
                team => team is not null ? Results.Ok(team) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<TeamResponse>()
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