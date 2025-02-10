using Api.Assignments.Contracts;
using Database.Models;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Assignments;

public class CreateAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateAssignment_ShouldCreateAssignment_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();

        var request = new CreateAssignmentRequest
        {
            Name = "Frontend project",
            DueDate = new DateTime(2025, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            Published = true,
            CollaborationType = CollaborationType.Individual,
            Mandatory = true,
            GradingType = GradingType.LetterGrading,
            Description = "Create a frontend project with svelte",
            CourseId = course.Id,
            Fields = [
                new NewAssignmentFieldRequest
                {
                    Name = "Project title",
                    Type = AssignmentDataType.String
                }
            ],
        };

        var response = await Client.PostAsJsonAsync($"assignments", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateAssignment_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var request = new CreateAssignmentRequest
        {
            Name = "Frontend project",
            DueDate = new DateTime(2025, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            Published = true,
            CollaborationType = CollaborationType.Individual,
            Mandatory = true,
            GradingType = GradingType.LetterGrading,
            Description = "Create a frontend project with svelte",
            CourseId = Guid.NewGuid(),
            Fields = [
                new NewAssignmentFieldRequest
                {
                    Name = "Project title",
                    Type = AssignmentDataType.String
                }
            ],
        };

        var response = await Client.PostAsJsonAsync($"assignments", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateAssignment_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();

        var request = new CreateAssignmentRequest
        {
            Name = "",
            DueDate = default,
            Published = true,
            CollaborationType = (CollaborationType)100,
            Mandatory = true,
            GradingType = (GradingType)100,
            Description = null!,
            CourseId = course.Id,
            Fields = [
                new NewAssignmentFieldRequest
                {
                    Name = "",
                    Type = (AssignmentDataType)100
                }
            ],
        };

        var response = await Client.PostAsJsonAsync($"assignments", request);

        await Verify(response);
    }
}