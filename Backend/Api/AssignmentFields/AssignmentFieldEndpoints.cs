using Api.AssignmentFields.Contracts;
using Api.Assignments.Contracts;

namespace Api.AssignmentFields;

public static class AssignmentFieldEndpoints
{
    public static void MapAssignmentFieldEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("AssignmentFields");

        group.MapGet("assignment-fields", async (IAssignmentFieldService assignmentFieldService) =>
        {
            var fields = await assignmentFieldService.GetAll();
            return Results.Ok(fields);
        })
        .Produces<IEnumerable<AssignmentFieldResponse>>()
        .WithName("GetAllAssignmentFields")
        .WithSummary("Get all assignment fields");

        group.MapGet("/assignments/{assignmentId:guid}/fields", async (IAssignmentFieldService assignmentFieldService, Guid assignmentId) =>
        {
            var fields = await assignmentFieldService.GetAllByAssignment(assignmentId);
            return fields is not null ? Results.Ok(fields) : Results.NotFound();
        })
        .Produces<IEnumerable<AssignmentFieldResponse>>()
        .WithName("GetAllAssignmentFieldsByAssignment")
        .WithSummary("Get all assignment fields by assignment id");

        group.MapPost("assignment-fields", async (IAssignmentFieldService assignmentFieldService, CreateAssignmentFieldRequest request) =>
        {
            var result = await assignmentFieldService.Create(request);

            return result.Match
            (
                field => field is not null ? Results.CreatedAtRoute
                    (
                        "GetAllAssignmentFieldsByAssignment",
                        new { assignmentId = field.AssignmentId },
                        field
                    ) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<AssignmentFieldResponse>()
        .WithName("CreateAssignmentField")
        .WithSummary("Create new assignment field for assignment");

        group.MapPost("assignment-fields/bulk-add", async (IAssignmentFieldService assignmentFieldService, CreateAssignmentFieldsRequest request) =>
        {
            var result = await assignmentFieldService.Create(request);

            return result.Match
            (
                fields => fields is not null ? Results.Ok(fields) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<IEnumerable<AssignmentFieldResponse>>()
        .WithName("CreateAssignmentFields")
        .WithSummary("Create new list of assignment field for assignment");

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
