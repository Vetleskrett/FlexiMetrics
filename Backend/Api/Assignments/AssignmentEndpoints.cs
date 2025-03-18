using Api.Assignments.Contracts;
using Database.Models;

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
        .RequireAuthorization("Course")
        .WithSummary("Get all assignments by course id");

        group.MapGet("students/{studentId:guid}/course/{courseId:guid}/assignments", async (IAssignmentService assignmentService, Guid studentId, Guid courseId) =>
        {
            var result = await assignmentService.GetAllByStudentCourse(studentId, courseId);
            return result.MapToResponse(assignments => Results.Ok(assignments));
        })
        .Produces<IEnumerable<StudentAssignmentResponse>>()
        .WithName("GetAllAssignmentsByStudentCourse")
        .RequireAuthorization("Course")
        .WithSummary("Get all assignments by student id and course id");

        group.MapGet("courses/{courseId:guid}/teams/{teamId:guid}/assignments", async (IAssignmentService assignmentService, Guid courseId, Guid teamId) =>
        {
            var result = await assignmentService.GetAllByTeamCourse(courseId, teamId);
            return result.MapToResponse(assignments => Results.Ok(assignments));
        })
        .Produces<IEnumerable<StudentAssignmentResponse>>()
        .WithName("GetAllAssignmentsByTeamCourse")
        .RequireAuthorization("Course")
        .WithSummary("Get all assignments by team id and course id");

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
                new { id = assignment.Id },
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

        group.MapPut("assignments/{assignmentId:guid}/publish", async (IAssignmentService assignmentService, Guid assignmentId) =>
        {
            var result = await assignmentService.Publish(assignmentId);
            return result.MapToResponse(assignment => Results.Ok(assignment));
        })
      .Produces<AssignmentResponse>()
      .WithName("PublishAssignment")
      .RequireAuthorization("TeacherForAssignment")
      .WithSummary("Publish assignment by id");

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
