using Api.Assignments.Contracts;
using Api.Validation;

namespace Api.Assignments;

public static class AssignmentEndpoints
{
    public static void MapAssignmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("assignments").WithTags("Assignments");

        group.MapGet("/", async (IAssignmentService assignmentService) =>
        {
            var courses = await assignmentService.GetAll();
            var coursesResponse = courses.MapToResponse();
            return Results.Ok(coursesResponse);
        })
        .Produces<IEnumerable<AssignmentResponse>>()
        .WithName("GetAllAssignments")
        .WithSummary("Get all assignments");

        group.MapGet("/course/{courseId:guid}", async (IAssignmentService assignmentService, Guid courseId) =>
        {
            var courses = await assignmentService.GetAllByCourse(courseId);
            var coursesResponse = courses.MapToResponse();
            return Results.Ok(coursesResponse);
        })
        .Produces<IEnumerable<AssignmentResponse>>()
        .WithName("GetAllAssignmentsByCourse")
        .WithSummary("Get all assignments by course");

        group.MapGet("/{id:guid}", async (IAssignmentService assignmentService, Guid id) =>
        {
            var result = await assignmentService.GetById(id);
            if (result is not null)
            {
                return Results.Ok(result.MapToResponse());
            }
            else
            {
                return Results.NotFound();
            }
        })
        .Produces<AssignmentResponse>()
        .WithName("GetAssignment")
        .WithSummary("Get assignment by id");

        group.MapGet("/{id:guid}/fields", async (IAssignmentService assignmentService, Guid id) =>
        {
            var result = await assignmentService.GetByIdWithFields(id);
            if (result is not null)
            {
                return Results.Ok(result.Value.Item1.MapToResponse(result.Value.Item2));
            }
            else
            {
                return Results.NotFound();
            }
        })
       .Produces<AssignmentResponse>()
       .WithName("GetAssignmentWithFields")
       .WithSummary("Get assignment and fields by id");

        group.MapPost("/", async (IAssignmentService assignmentService, CreateAssignmentRequest request) =>
        {
            var assignment = request.MapToAssignment();
            var fields = request.Fields.MapToAssignmentField();
            var result = await assignmentService.Create(assignment, fields.ToList());

            return result.Match
            (
                assignment => Results.CreatedAtRoute
                    (
                        "GetAssignment",
                        new { id = assignment.Item1.Id },
                        assignment.Item1.MapToResponse(assignment.Item2)
                    ),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<AssignmentResponse>()
        .WithName("CreateAssignment")
        .WithSummary("Create new assignment with fields");

        group.MapPut("/{id:guid}", async (IAssignmentService assignmentService, Guid id, UpdateAssignmentRequest request) =>
        {
            var assignment = request.MapToAssignment(id);
            var fields = request.Fields?.MapToAssignmentField();
            var result = await assignmentService.Update(assignment, fields?.ToList());

            return result.Match
            (
                assignment =>
                {
                    if (assignment is not null)
                    {
                        return Results.Ok(assignment.Value.Item1.MapToResponse(assignment.Value.Item2));
                    }
                    else
                    {
                        return Results.NotFound();
                    }
                },
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<AssignmentResponse>()
        .WithName("UpdateAssignment")
        .WithSummary("Update assignment by id, optionally with fields");

        group.MapDelete("/{id:guid}", async (IAssignmentService assignmentService, Guid id) =>
        {
            var deleted = await assignmentService.DeleteById(id);
            return deleted ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteAssignment")
        .WithSummary("Delete assignment by id");
    }
}
