using Api.AssignmentFields.Contracts;
using Api.Assignments.Contracts;

namespace Api.AssignmentFields;

public static class AssignmentFieldEndpoints
{
    public static void MapAssignmentFieldEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("AssignmentFields").RequireAuthorization();

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
        .RequireAuthorization("Assignment")
        .WithSummary("Get all assignment fields by assignment id");

        group.MapPut("assignments/{assignmentId:guid}/fields", async (IAssignmentFieldService assignmentFieldService, Guid assignmentId, UpdateAssignmentFieldsRequest request) =>
        {
            var result = await assignmentFieldService.Update(request, assignmentId);
            return result.MapToResponse(field => Results.Ok(field));
        })
        .Produces<AssignmentResponse>()
        .WithName("UpdateAssignmentFields")
        .RequireAuthorization("TeacherForAssignment")
        .WithSummary("Update assignment fields by assignment id");

        group.MapDelete("assignment-fields/{assignmentFieldId:guid}", async (IAssignmentFieldService assignmentFieldService, Guid assignmentFieldId) =>
        {
            var result = await assignmentFieldService.DeleteById(assignmentFieldId);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteAssignmentField")
        .RequireAuthorization("TeacherForAssignmentField")
        .WithSummary("Delete assignment field by id");
    }
}
