using Api.Assignments.Contracts;
using Database.Models;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Assignments;

public class UpdateAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateAssignment_ShouldUpdateAssignment_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();

        var assignment = ModelFactory.CreateAssignment(course.Id);

        await DbContext.SaveChangesAsync();

        var request = new UpdateAssignmentRequest
        {
            Name = "Backend project",
            DueDate = new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            Published = false,
            CollaborationType = CollaborationType.Teams,
            Mandatory = false,
            GradingType = GradingType.ApprovalGrading,
            Description = "Create a backend project with .net",
        };

        var response = await Client.PutAsJsonAsync($"assignments/{assignment.Id}", request);

        await Verify(response);
    }

    [Fact]
    public async Task UpdateAssignment_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var course = ModelFactory.CreateCourse();

        var assignment = ModelFactory.CreateAssignment(course.Id);

        await DbContext.SaveChangesAsync();

        var request = new UpdateAssignmentRequest
        {
            Name = "",
            DueDate = default,
            Published = true,
            CollaborationType = (CollaborationType)100,
            Mandatory = true,
            GradingType = (GradingType)100,
            Description = null!,
        };

        var response = await Client.PutAsJsonAsync($"assignments/{assignment.Id}", request);

        await Verify(response);
    }

    [Fact]
    public async Task UpdateAssignment_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var request = new UpdateAssignmentRequest
        {
            Name = "Backend project",
            DueDate = new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            Published = false,
            CollaborationType = CollaborationType.Teams,
            Mandatory = false,
            GradingType = GradingType.ApprovalGrading,
            Description = "Create a backend project with .net",
        };

        var assignmentId = Guid.NewGuid();

        var response = await Client.PutAsJsonAsync($"assignments/{assignmentId}", request);

        await Verify(response);
    }
}