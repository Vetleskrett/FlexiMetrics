using Api.AssignmentFields.Contracts;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Api.Tests.Integration.AssignmentFields;

public class UpdateAssignmentFieldsTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateAssignmentFields_ShouldUpdateAssignmentFields_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        var fieldToBeDeleted = ModelFactory.CreateAssignmentField(assignment.Id);
        var fieldToBeUpdated = ModelFactory.CreateAssignmentField(assignment.Id);
        await DbContext.SaveChangesAsync();

        var request = new UpdateAssignmentFieldsRequest
        {
            Fields =
            [
                new AssignmentFieldRequest
                {
                    Name = "New field",
                    Type = AssignmentDataType.Boolean,
                },
                new AssignmentFieldRequest
                {
                    Id = fieldToBeUpdated.Id,
                    Name = "Updated field",
                    Type = AssignmentDataType.Integer,
                }
            ]

        };

        var response = await Client.PutAsJsonAsync($"assignments/{assignment.Id}/fields", request);

        await Verify(response);
        Assert.True(await DbContext.AssignmentFields.AnyAsync(f =>
            f.AssignmentId == assignment.Id &&
            f.Name == request.Fields[0].Name &&
            f.Type == request.Fields[0].Type
        ));
        Assert.True(await DbContext.AssignmentFields.AnyAsync(f =>
            f.AssignmentId == assignment.Id &&
            f.Id == request.Fields[1].Id &&
            f.Name == request.Fields[1].Name &&
            f.Type == request.Fields[1].Type
        ));
        Assert.False(await DbContext.AssignmentFields.AnyAsync(f =>
            f.Id == fieldToBeDeleted.Id
        ));
    }

    [Fact]
    public async Task UpdateAssignmentFields_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var request = new UpdateAssignmentFieldsRequest
        {
            Fields =
            [
                new AssignmentFieldRequest
                {
                    Name = "Project title",
                    Type = AssignmentDataType.ShortText,
                }
            ]

        };
        var assignmentId = Guid.NewGuid();

        var response = await Client.PutAsJsonAsync($"assignments/{assignmentId}/fields", request);

        await Verify(response);
    }

    [Fact]
    public async Task UpdateAssignmentFields_ShouldReturnNotFound_WhenInvalidAssignmentField()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        var otherAssignment = ModelFactory.CreateAssignment(course.Id);
        var otherField = ModelFactory.CreateAssignmentField(otherAssignment.Id);
        await DbContext.SaveChangesAsync();

        var request = new UpdateAssignmentFieldsRequest
        {
            Fields =
            [
                new AssignmentFieldRequest
                {
                    Id = otherField.Id,
                    Name = "Project title",
                    Type = AssignmentDataType.ShortText,
                }
            ]

        };

        var response = await Client.PutAsJsonAsync($"assignments/{assignment.Id}/fields", request);

        await Verify(response);
    }

    [Fact]
    public async Task UpdateAssignmentFields_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        await DbContext.SaveChangesAsync();

        var request = new UpdateAssignmentFieldsRequest
        {
            Fields =
            [
                new AssignmentFieldRequest
                {
                    Name = "",
                    Type = (AssignmentDataType)100,
                }
            ]

        };

        var response = await Client.PutAsJsonAsync($"assignments/{assignment.Id}/fields", request);

        await Verify(response);
    }
}