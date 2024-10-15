using Api.Assignments;
using Api.Assignments.Contracts;
using Api.Validation;

namespace Api.Assignments
{
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

            group.MapGet("/{id:guid}/variables", async (IAssignmentService assignmentService, Guid id) =>
            {
                var result = await assignmentService.GetByIdWithVariables(id);
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
           .WithName("GetAssignmentWithVariables")
           .WithSummary("Get assignment and variables by id");

            group.MapPost("/", async (IAssignmentService assignmentService, CreateAssignmentRequest request) =>
            {
                var assignment = request.MapToAssignment();
                var variables = request.Variables.MapToAssignmentVariable();
                var result = await assignmentService.Create(assignment, variables.ToList());

                return result.Match
                (
                    assignment => Results.CreatedAtRoute
                        (
                            "GetAssignment",
                            new { id = assignment.Item1.Id },
                            assignment.Item1.MapToResponse(assignment.Item2)
                        ),
                    failed => Results.BadRequest(failed.MapToResponse())
                );
            })
            .Produces<AssignmentResponse>()
            .WithName("CreateAssignment")
            .WithSummary("Create new assignment with variables");

            group.MapPut("/{id:guid}", async (IAssignmentService assignmentService, Guid id, UpdateAssignmentRequest request) =>
            {
                var assignment = request.MapToAssignment(id);
                var variables = request.Variables?.MapToAssignmentVariable();
                var result = await assignmentService.Update(assignment, variables?.ToList());

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
                    failed => Results.BadRequest(failed.MapToResponse())
                );
            })
            .Produces<AssignmentResponse>()
            .WithName("UpdateAssignment")
            .WithSummary("Update assignment by id, optionally with variables");

            group.MapDelete("/{id:guid}", async (IAssignmentService assignmentService, Guid id) =>
            {
                var deleted = await assignmentService.DeleteById(id);
                return deleted ? Results.Ok() : Results.NotFound();
            })
            .WithName("DeleteAssignment")
            .WithSummary("Delete assignment by id");
        }
    }
}
