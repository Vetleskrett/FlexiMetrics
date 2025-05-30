﻿using Api.Teams.Contracts;

namespace Api.Teams;

public static class TeamEndpoints
{
    public static void MapTeamEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Teams").RequireAuthorization();

        group.MapGet("teams", async (ITeamService teamService) =>
        {
            var result = await teamService.GetAll();
            return result.MapToResponse(teams => Results.Ok(teams));
        })
        .Produces<IEnumerable<TeamResponse>>()
        .WithName("GetAllTeams")
        .RequireAuthorization("Admin")
        .WithSummary("Get all teams");

        group.MapGet("courses/{courseId:guid}/teams", async (ITeamService teamService, Guid courseId) =>
        {
            var result = await teamService.GetAllByCourse(courseId);
            return result.MapToResponse(teams => Results.Ok(teams));
        })
        .Produces<IEnumerable<TeamResponse>>()
        .WithName("GetAllTeamsByCourse")
        .RequireAuthorization("Course")
        .WithSummary("Get all teams by course id");

        group.MapGet("teams/{teamId:guid}", async (ITeamService teamService, Guid teamId) =>
        {
            var result = await teamService.GetById(teamId);
            return result.MapToResponse(team => Results.Ok(team));
        })
        .Produces<TeamResponse>()
        .WithName("GetTeam")
        .WithSummary("Get team by id");

        group.MapGet("students/{studentId:guid}/courses/{courseId:guid}/teams", async (ITeamService teamService, Guid studentId, Guid courseId) =>
        {
            var result = await teamService.GetByStudentCourse(studentId, courseId);
            return result.MapToResponse(team => Results.Ok(team));
        })
        .Produces<TeamResponse>()
        .WithName("GetTeamByStudentCourse")
        .RequireAuthorization("Course")
        .WithSummary("Get team by student id and course id");

        group.MapPost("teams", async (ITeamService teamService, CreateTeamsRequest request) =>
        {
            var result = await teamService.Create(request);
            return result.MapToResponse(teams => Results.CreatedAtRoute
            (
                "GetAllTeamsByCourse",
                new { courseId = request.CourseId },
                teams
            ));
        })
        .Produces<IEnumerable<TeamResponse>>()
        .WithName("CreateTeams")
        .RequireAuthorization("Teacher")
        .WithSummary("Create new teams");

        group.MapPost("teams/bulk", async (ITeamService teamService, BulkAddStudentsToTeamsRequest request) =>
        {
            var result = await teamService.BulkAddToTeams(request);
            return result.MapToResponse(teams => Results.Ok(teams));
        })
        .Produces<IEnumerable<TeamResponse>>()
        .WithName("BulkAddStudentsToTeams")
        .RequireAuthorization("Teacher")
        .WithSummary("Bulk add students to teams");

        group.MapPost("teams/{teamId:guid}/students", async (ITeamService teamService, Guid teamId, AddStudentToTeamRequest request) =>
        {
            var result = await teamService.AddToTeam(teamId, request);
            return result.MapToResponse(team => Results.Ok(team));
        })
        .Produces<TeamResponse>()
        .WithName("AddStudentEmailToTeam")
        .RequireAuthorization("TeacherForTeam")
        .WithSummary("Add student to team");

        group.MapPost("teams/{teamId:guid}/students/{studentId:guid}", async (ITeamService teamService, Guid teamId, Guid studentId) =>
        {
            var result = await teamService.AddToTeam(teamId, studentId);
            return result.MapToResponse(team => Results.Ok(team));
        })
        .Produces<TeamResponse>()
        .WithName("AddStudentIdToTeam")
        .WithSummary("Add student to team");

        group.MapDelete("teams/{teamId:guid}/students/{studentId:guid}", async (ITeamService teamService, Guid teamId, Guid studentId) =>
        {
            var result = await teamService.RemoveFromTeam(teamId, studentId);
            return result.MapToResponse(team => Results.Ok(team));
        })
        .Produces<TeamResponse>()
        .WithName("RemoveStudentFromTeam")
        .RequireAuthorization("TeacherForTeam")
        .WithSummary("Remove student from team");

        group.MapDelete("teams/{teamId:guid}", async (ITeamService teamService, Guid teamId) =>
        {
            var result = await teamService.DeleteById(teamId);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteTeam")
        .RequireAuthorization("TeacherForTeam")
        .WithSummary("Delete team by id");
    }
}