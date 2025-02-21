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

        group.MapGet("assignments/{assignmentId:guid}/fields", async (IAssignmentFieldService assignmentFieldService, Guid assignmentId) =>
        {
            var result = await assignmentFieldService.GetAllByAssignment(assignmentId);
            return result.MapToResponse(fields => Results.Ok(fields));
        })
        .Produces<IEnumerable<AssignmentFieldResponse>>()
        .WithName("GetAllAssignmentFieldsByAssignment")
        .WithSummary("Get all assignment fields by assignment id");

        group.MapPut("assignments/{assignmentId:guid}/fields", async (IAssignmentFieldService assignmentFieldService, Guid assignmentId, UpdateAssignmentFieldsRequest request) =>
        {
            var result = await assignmentFieldService.Update(request, assignmentId);
            return result.MapToResponse(field => Results.Ok(field));
        })
        .Produces<AssignmentResponse>()
        .WithName("UpdateAssignmentFields")
        .WithSummary("Update assignment fields by assignment id");

        group.MapDelete("assignment-fields/{id:guid}", async (IAssignmentFieldService assignmentFieldService, Guid id) =>
        {
            var result = await assignmentFieldService.DeleteById(id);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteAssignmentField")
        .WithSummary("Delete assignment field by id");
    }
}
