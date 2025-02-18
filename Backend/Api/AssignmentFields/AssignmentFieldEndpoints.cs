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
            var result = await assignmentFieldService.GetAll();
            return result.MapToResponse(fields => Results.Ok(fields));
        })
        .Produces<IEnumerable<AssignmentFieldResponse>>()
        .WithName("GetAllAssignmentFields")
        .WithSummary("Get all assignment fields");

        group.MapGet("/assignments/{assignmentId:guid}/fields", async (IAssignmentFieldService assignmentFieldService, Guid assignmentId) =>
        {
            var result = await assignmentFieldService.GetAllByAssignment(assignmentId);
            return result.MapToResponse(fields => Results.Ok(fields));
        })
        .Produces<IEnumerable<AssignmentFieldResponse>>()
        .WithName("GetAllAssignmentFieldsByAssignment")
        .WithSummary("Get all assignment fields by assignment id");

        group.MapPost("assignment-fields", async (IAssignmentFieldService assignmentFieldService, CreateAssignmentFieldRequest request) =>
        {
            var result = await assignmentFieldService.Create(request);
            return result.MapToResponse(field => Results.CreatedAtRoute
            (
                "GetAllAssignmentFieldsByAssignment",
                new { assignmentId = field.AssignmentId },
                field
            ));
        })
        .Produces<AssignmentFieldResponse>()
        .WithName("CreateAssignmentField")
        .WithSummary("Create new assignment field for assignment");

        group.MapPost("assignment-fields/bulk-add", async (IAssignmentFieldService assignmentFieldService, CreateAssignmentFieldsRequest request) =>
        {
            var result = await assignmentFieldService.Create(request);
            return result.MapToResponse(fields => Results.Ok(fields));
        })
        .Produces<IEnumerable<AssignmentFieldResponse>>()
        .WithName("CreateAssignmentFields")
        .WithSummary("Create new list of assignment field for assignment");

        group.MapPut("assignment-fields/{id:guid}", async (IAssignmentFieldService assignmentFieldService, Guid id, UpdateAssignmentFieldRequest request) =>
        {
            var result = await assignmentFieldService.Update(request, id);
            return result.MapToResponse(field => Results.Ok(field));
        })
        .Produces<AssignmentResponse>()
        .WithName("UpdateAssignmentField")
        .WithSummary("Update assignment field by id");

        group.MapDelete("assignment-fields/{id:guid}", async (IAssignmentFieldService assignmentFieldService, Guid id) =>
        {
            var result = await assignmentFieldService.DeleteById(id);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteAssignmentField")
        .WithSummary("Delete assignment field by id");
    }
}
