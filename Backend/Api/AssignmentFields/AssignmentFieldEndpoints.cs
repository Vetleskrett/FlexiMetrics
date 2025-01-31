﻿using Api.AssignmentFields.Contracts;
using Api.Assignments.Contracts;

namespace Api.AssignmentFields;

public static class AssignmentFieldEndpoints
{
    public static void MapAssignmentFieldEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("AssignmentFields");

        group.MapGet("/assignments/{assignmentId:guid}/fields", async (IAssignmentFieldService assignmentFieldService, Guid assignmentId) =>
        {
            var fields = await assignmentFieldService.GetAllByAssignment(assignmentId);
            return Results.Ok(fields);
        })
        .Produces<IEnumerable<AssignmentFieldResponse>>()
        .WithName("GetAllAssignmentFieldsByAssignment")
        .WithSummary("Get all assignment fields by assignment id");

        group.MapPost("assignment-fields", async (IAssignmentFieldService assignmentFieldService, CreateAssignmentFieldRequest request) =>
        {
            var result = await assignmentFieldService.Create(request);

            return result.Match
            (
                field => Results.CreatedAtRoute
                    (
                        "GetAllAssignmentFieldsByAssignment",
                        new { assignmentId = field.AssignmentId },
                        field
                    ),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<AssignmentResponse>()
        .WithName("CreateAssignmentField")
        .WithSummary("Create new assignment with fields");

        group.MapPut("assignment-fields/{id:guid}", async (IAssignmentFieldService assignmentFieldService, Guid id, UpdateAssignmentFieldRequest request) =>
        {
            var result = await assignmentFieldService.Update(request, id);

            return result.Match
            (
                field => field is not null ? Results.Ok(field) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<AssignmentResponse>()
        .WithName("UpdateAssignmentField")
        .WithSummary("Update assignment field by id");

        group.MapDelete("assignment-fields/{id:guid}", async (IAssignmentFieldService assignmentFieldService, Guid id) =>
        {
            var deleted = await assignmentFieldService.DeleteById(id);
            return deleted ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteAssignmentField")
        .WithSummary("Delete assignment field by id");
    }
}
