using Api.AssignmentFields.Contracts;
using Database.Models;
using System.Net.Http.Json;

namespace Api.Tests.Integration.AssignmentFields;

public class UpdateAssignmentFieldTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateAssignmentField_ShouldUpdateAssignmentField_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();

        var assignment = ModelFactory.CreateAssignment(course.Id);

        var field = ModelFactory.CreateAssignmentField(assignment.Id);

        await DbContext.SaveChangesAsync();

        var request = new UpdateAssignmentFieldRequest
        {
            Name = "Member count",
            Type = AssignmentDataType.Integer,
        };

        var response = await Client.PutAsJsonAsync($"assignment-fields/{field.Id}", request);

        await Verify(response);
    }

    [Fact]
    public async Task UpdateAssignmentField_ShouldReturnNotFound_WhenInvalidAssignmentField()
    {
        var request = new UpdateAssignmentFieldRequest
        {
            Name = "Member count",
            Type = AssignmentDataType.Integer,
        };
        var id = Guid.NewGuid();

        var response = await Client.PutAsJsonAsync($"assignment-fields/{id}", request);

        await Verify(response);
    }

    [Fact]
    public async Task UpdateAssignmentField_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var course = ModelFactory.CreateCourse();

        var assignment = ModelFactory.CreateAssignment(course.Id);

        var field = ModelFactory.CreateAssignmentField(assignment.Id);

        await DbContext.SaveChangesAsync();

        var request = new UpdateAssignmentFieldRequest
        {
            Name = "",
            Type = (AssignmentDataType)100,
        };

        var response = await Client.PutAsJsonAsync($"assignment-fields/{field.Id}", request);

        await Verify(response);
    }
}