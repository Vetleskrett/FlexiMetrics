using Api.Assignments.Contracts;
using Database.Models;
using Microsoft.EntityFrameworkCore;
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
                new CreateAssignmentFieldRequest
                {
                    Name = "Project title",
                    Type = AssignmentDataType.String
                }
            ],
        };

        var response = await Client.PostAsJsonAsync($"assignments", request);

        await Verify(response);
        Assert.True(await DbContext.Assignments.AnyAsync(a =>
            a.CourseId == course.Id &&
            a.Name == request.Name &&
            a.DueDate == request.DueDate &&
            a.Published == request.Published &&
            a.CollaborationType == request.CollaborationType &&
            a.Mandatory == request.Mandatory &&
            a.GradingType == request.GradingType &&
            a.Description == request.Description &&
            a.Fields!.Any(f =>
                f.Name == request.Fields[0].Name &&
                f.Type == request.Fields[0].Type
            )
        ));
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
                new CreateAssignmentFieldRequest
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
                new CreateAssignmentFieldRequest
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