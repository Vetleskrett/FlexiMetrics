using Api.Teams.Contracts;
using Api.Validation;

namespace Api.Teams;

public static class TeamEndpoints
{
    public static void MapTeamEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("teams").WithTags("Teams");

        group.MapGet("/", async (ITeamService teamService) =>
        {
            var teams = await teamService.GetAll();
            var teamsResponse = teams.MapToResponse();
            return Results.Ok(teamsResponse);
        })
        .Produces<IEnumerable<TeamResponse>>()
        .WithName("GetAllTeams")
        .WithSummary("Get all teams");

        group.MapGet("/{id:guid}", async (ITeamService teamService, Guid id) =>
        {
            var result = await teamService.GetById(id);
            if (result is not null)
            {
                return Results.Ok(result.MapToResponse());
            }
            else
            {
                return Results.NotFound();
            }
        })
        .Produces<TeamResponse>()
        .WithName("GetTeam")
        .WithSummary("Get team by id");

        group.MapPost("/", async (ITeamService teamService, CreateTeamRequest request) =>
        {
            var team = request.MapToTeam();
            var result = await teamService.Create(team);

            return result.Match
            (
                team => Results.CreatedAtRoute
                    (
                        "GetCourse",
                        new { id = team.Id },
                        team.MapToResponse()
                    ),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<TeamResponse>()
        .WithName("CreateTeam")
        .WithSummary("Create new team");

        group.MapPut("/{id:guid}", async (ITeamService teamService, Guid id, UpdateTeamRequest request) =>
        {
            var team = request.MapToTeam(id);
            var result = await teamService.Update(team);

            return result.Match
            (
                team =>
                {
                    if (team is not null)
                    {
                        return Results.Ok(team.MapToResponse());
                    }
                    else
                    {
                        return Results.NotFound();
                    }
                },
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<TeamResponse>()
        .WithName("UpdateTeam")
        .WithSummary("Update team by id");

        group.MapDelete("/{id:guid}", async (ITeamService teamService, Guid id) =>
        {
            var deleted = await teamService.DeleteById(id);
            return deleted ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteTeam")
        .WithSummary("Delete team by id");
    }
}
