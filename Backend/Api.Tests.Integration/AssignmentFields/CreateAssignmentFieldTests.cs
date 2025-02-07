using Api.AssignmentFields.Contracts;
using Database.Models;
using System.Net.Http.Json;

namespace Api.Tests.Integration.AssignmentFields;

public class CreateAssignmentFieldTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateAssignmentField_ShouldCreateAssignmentField_WhenValidRequest()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var assignment = ModelFactory.GetValidAssignment(course.Id);
        DbContext.Assignments.Add(assignment);

        await DbContext.SaveChangesAsync();

        var request = new CreateAssignmentFieldRequest
        {
            Name = "Project title",
            Type = AssignmentDataType.String,
            AssignmentId = assignment.Id,
        };

        var response = await Client.PostAsJsonAsync("assignment-fields", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateAssignmentField_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var request = new CreateAssignmentFieldRequest
        {
            Name = "Project title",
            Type = AssignmentDataType.String,
            AssignmentId = Guid.NewGuid(),
        };

        var response = await Client.PostAsJsonAsync("assignment-fields", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateAssignmentField_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var assignment = ModelFactory.GetValidAssignment(course.Id);
        DbContext.Assignments.Add(assignment);

        await DbContext.SaveChangesAsync();

        var request = new CreateAssignmentFieldRequest
        {
            Name = "",
            Type = (AssignmentDataType)100,
            AssignmentId = assignment.Id,
        };

        var response = await Client.PostAsJsonAsync("assignment-fields", request);

        await Verify(response);
    }
}