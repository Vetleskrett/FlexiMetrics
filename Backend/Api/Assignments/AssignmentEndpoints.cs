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

        group.MapGet("teacher/courses/{courseId:guid}/assignments", async (IAssignmentService assignmentService, Guid courseId) =>
        {
            var result = await assignmentService.GetAllByCourse(courseId, true);
            return result.MapToResponse(assignments => Results.Ok(assignments));
        })
        .Produces<IEnumerable<AssignmentResponse>>()
        .WithName("GetAllTeacherAssignmentsByCourse")
        .RequireAuthorization("TeacherInCourse")
        .WithSummary("Get all teacher assignments by course id");

        group.MapGet("student/courses/{courseId:guid}/assignments", async (IAssignmentService assignmentService, Guid courseId) =>
        {
            var result = await assignmentService.GetAllByCourse(courseId, false);
            return result.MapToResponse(assignments => Results.Ok(assignments));
        })
        .Produces<IEnumerable<AssignmentResponse>>()
        .WithName("GetAllStudentAssignmentsByCourse")
        .RequireAuthorization("StudentInCourse")
        .WithSummary("Get all student assignments by course id");

        group.MapGet("assignments/{assignmentId:guid}", async (IAssignmentService assignmentService, Guid assignmentId) =>
        {
            var result = await assignmentService.GetById(assignmentId);
            return result.MapToResponse(assignment => Results.Ok(assignment));
        })
        .Produces<AssignmentResponse>()
        .WithName("GetAssignment")
        .RequireAuthorization("Assignment")
        .WithSummary("Get assignment by id");

        group.MapPost("assignments", async (IAssignmentService assignmentService, CreateAssignmentRequest request) =>
        {
            var result = await assignmentService.Create(request);
            return result.MapToResponse(assignment => Results.CreatedAtRoute
            (
                "GetAssignment",
                new { assignmentId = assignment.Id },
                assignment
            ));
        })
        .Produces<AssignmentResponse>()
        .WithName("CreateAssignment")
        .RequireAuthorization("Teacher")
        .WithSummary("Create new assignment with fields");

        group.MapPut("assignments/{assignmentId:guid}", async (IAssignmentService assignmentService, Guid assignmentId, UpdateAssignmentRequest request) =>
        {
            var result = await assignmentService.Update(request, assignmentId);
            return result.MapToResponse(assignment => Results.Ok(assignment));
        })
        .Produces<AssignmentResponse>()
        .WithName("UpdateAssignment")
        .RequireAuthorization("TeacherForAssignment")
        .WithSummary("Update assignment by id");

        group.MapDelete("assignments/{assignmentId:guid}", async (IAssignmentService assignmentService, Guid assignmentId) =>
        {
            var result = await assignmentService.DeleteById(assignmentId);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteAssignment")
        .RequireAuthorization("TeacherForAssignment")
        .WithSummary("Delete assignment by id");
    }
}
