using Api.Assignments.Contracts;

namespace Api.Assignments;

public static class AssignmentEndpoints
{
    public static void MapAssignmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Assignments").RequireAuthorization();

        group.MapGet("assignments", async (IAssignmentService assignmentService) =>
        {
            var result = await assignmentService.GetAll();
            return result.MapToResponse(assignments => Results.Ok(assignments));
        })
        .Produces<IEnumerable<AssignmentResponse>>()
        .WithName("GetAllAssignments")
        .WithSummary("Get all assignments");

        group.MapGet("course/{courseId:guid}/assignments", async (IAssignmentService assignmentService, Guid courseId) =>
        {
            var result = await assignmentService.GetAllByCourse(courseId);
            return result.MapToResponse(assignments => Results.Ok(assignments));
        })
        .Produces<IEnumerable<AssignmentResponse>>()
        .WithName("GetAllAssignmentsByCourse")
        .WithSummary("Get all assignments by course id");

        group.MapGet("students/{studentId:guid}/course/{courseId:guid}/assignments", async (IAssignmentService assignmentService, Guid studentId, Guid courseId) =>
        {
            var result = await assignmentService.GetAllByStudentCourse(studentId, courseId);
            return result.MapToResponse(assignments => Results.Ok(assignments));
        })
        .Produces<IEnumerable<StudentAssignmentResponse>>()
        .WithName("GetAllAssignmentsByStudentCourse")
        .WithSummary("Get all assignments by student id and course id");

        group.MapGet("courses/{courseId:guid}/teams/{teamId:guid}/assignments", async (IAssignmentService assignmentService, Guid courseId, Guid teamId) =>
        {
            var result = await assignmentService.GetAllByTeamCourse(courseId, teamId);
            return result.MapToResponse(assignments => Results.Ok(assignments));
        })
        .Produces<IEnumerable<StudentAssignmentResponse>>()
        .WithName("GetAllAssignmentsByTeamCourse")
        .WithSummary("Get all assignments by team id and course id");

        group.MapGet("assignments/{id:guid}", async (IAssignmentService assignmentService, Guid id) =>
        {
            var result = await assignmentService.GetById(id);
            return result.MapToResponse(assignment => Results.Ok(assignment));
        })
        .Produces<AssignmentResponse>()
        .WithName("GetAssignment")
        .WithSummary("Get assignment by id");

        group.MapPost("assignments", async (IAssignmentService assignmentService, CreateAssignmentRequest request) =>
        {
            var result = await assignmentService.Create(request);
            return result.MapToResponse(assignment => Results.CreatedAtRoute
            (
                "GetAssignment",
                new { id = assignment.Id },
                assignment
            ));
        })
        .Produces<AssignmentResponse>()
        .WithName("CreateAssignment")
        .WithSummary("Create new assignment with fields");

        group.MapPut("assignments/{id:guid}", async (IAssignmentService assignmentService, Guid id, UpdateAssignmentRequest request) =>
        {
            var result = await assignmentService.Update(request, id);
            return result.MapToResponse(assignment => Results.Ok(assignment));
        })
        .Produces<AssignmentResponse>()
        .WithName("UpdateAssignment")
        .WithSummary("Update assignment by id");

        group.MapPut("assignments/{id:guid}/publish", async (IAssignmentService assignmentService, Guid id) =>
        {
            var result = await assignmentService.Publish(id);
            return result.MapToResponse(assignment => Results.Ok(assignment));
        })
        .Produces<AssignmentResponse>()
        .WithName("PublishAssignment")
        .WithSummary("Publish assignment by id");

        group.MapDelete("assignments/{id:guid}", async (IAssignmentService assignmentService, Guid id) =>
        {
            var result = await assignmentService.DeleteById(id);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteAssignment")
        .WithSummary("Delete assignment by id");
    }
}
