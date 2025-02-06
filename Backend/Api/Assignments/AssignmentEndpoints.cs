using Api.Assignments.Contracts;
using Api.Validation;
using Database.Models;

namespace Api.Assignments;

public static class AssignmentEndpoints
{
    public static void MapAssignmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Assignments");

        group.MapGet("assignments", async (IAssignmentService assignmentService) =>
        {
            var assignments = await assignmentService.GetAll();
            return Results.Ok(assignments);
        })
        .Produces<IEnumerable<AssignmentResponse>>()
        .WithName("GetAllAssignments")
        .WithSummary("Get all assignments");

        group.MapGet("course/{courseId:guid}/assignments", async (IAssignmentService assignmentService, Guid courseId) =>
        {
            var assignments = await assignmentService.GetAllByCourse(courseId);
            return assignments is not null ? Results.Ok(assignments) : Results.NotFound();
        })
        .Produces<IEnumerable<AssignmentResponse>>()
        .WithName("GetAllAssignmentsByCourse")
        .WithSummary("Get all assignments by course id");

        group.MapGet("students/{studentId:guid}/course/{courseId:guid}/assignments", async (IAssignmentService assignmentService, Guid studentId, Guid courseId) =>
        {
            var result = await assignmentService.GetAllByStudentCourse(studentId, courseId);
            return result.Match
            (
                assignment => assignment is not null ? Results.Ok(assignment) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<IEnumerable<StudentAssignmentResponse>>()
        .WithName("GetAllAssignmentsByStudentCourse")
        .WithSummary("Get all assignments by student id and course id");

        group.MapGet("assignments/{id:guid}", async (IAssignmentService assignmentService, Guid id) =>
        {
            var assignment = await assignmentService.GetById(id);
            return assignment is not null ? Results.Ok(assignment) : Results.NotFound();
        })
        .Produces<AssignmentResponse>()
        .WithName("GetAssignment")
        .WithSummary("Get assignment by id");

        group.MapPost("assignments", async (IAssignmentService assignmentService, CreateAssignmentRequest request) =>
        {
            var result = await assignmentService.Create(request);

            return result.Match
            (
                assignment => assignment is not null ? Results.CreatedAtRoute
                    (
                        "GetAssignment",
                        new { id = assignment.Id },
                        assignment
                    ) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<AssignmentResponse>()
        .WithName("CreateAssignment")
        .WithSummary("Create new assignment with fields");

        group.MapPut("assignments/{id:guid}", async (IAssignmentService assignmentService, Guid id, UpdateAssignmentRequest request) =>
        {
            var result = await assignmentService.Update(request, id);

            return result.Match
            (
                assignment => assignment is not null ? Results.Ok(assignment) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<AssignmentResponse>()
        .WithName("UpdateAssignment")
        .WithSummary("Update assignment by id");

        group.MapDelete("assignments/{id:guid}", async (IAssignmentService assignmentService, Guid id) =>
        {
            var deleted = await assignmentService.DeleteById(id);
            return deleted ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteAssignment")
        .WithSummary("Delete assignment by id");
    }
}
