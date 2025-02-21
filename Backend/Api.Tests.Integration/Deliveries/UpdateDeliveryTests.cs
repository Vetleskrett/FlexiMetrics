using Api.Deliveries.Contracts;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.Tests.Integration.Deliveries;

public class UpdateDeliveryTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateDelivery_ShouldUpdateStudentDelivery_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);

        var textField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.ShortText);
        var intField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Integer);
        var floatField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Float);
        var boolField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Boolean);

        var delivery = ModelFactory.CreateStudentDeliveryWithFields(assignment.Id, [textField, intField, floatField, boolField], student.Id);

        await DbContext.SaveChangesAsync();

        var request = new UpdateDeliveryRequest
        {
            Fields = [
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = textField.Id,
                    Value = "Value"
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = intField.Id,
                    Value = 16
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = floatField.Id,
                    Value = 5.7
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = boolField.Id,
                    Value = false
                }
            ]
        };

        var response = await Client.PutAsJsonAsync($"deliveries/{delivery.Id}", request);

        await Verify(response);
        foreach (var field in request.Fields)
        {
            var jsonValue = JsonSerializer.SerializeToDocument(field.Value);
            Assert.True(await DbContext.Deliveries.AnyAsync(d =>
                d.Id == delivery.Id &&
                d.Fields!.Any(f =>
                    f.AssignmentFieldId == field.AssignmentFieldId &&
                    f.JsonValue == jsonValue
                )
            ));
        }
    }

    [Fact]
    public async Task UpdateDelivery_ShouldUpdateTeamDelivery_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var students = ModelFactory.CreateCourseStudents(course.Id, 2);
        var team = ModelFactory.CreateTeam(course.Id, students: students);
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);

        var textField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.ShortText);
        var intField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Integer);
        var floatField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Float);
        var boolField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Boolean);

        var delivery = ModelFactory.CreateTeamDeliveryWithFields(assignment.Id, [textField, intField, floatField, boolField], team.Id);

        await DbContext.SaveChangesAsync();

        var request = new UpdateDeliveryRequest
        {
            Fields = [
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = textField.Id,
                    Value = "Value"
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = intField.Id,
                    Value = 16
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = floatField.Id,
                    Value = 5.7
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = boolField.Id,
                    Value = false
                }
            ]
        };

        var response = await Client.PutAsJsonAsync($"deliveries/{delivery.Id}", request);

        await Verify(response);
        foreach (var field in request.Fields)
        {
            var jsonValue = JsonSerializer.SerializeToDocument(field.Value);
            Assert.True(await DbContext.Deliveries.AnyAsync(d =>
                d.Id == delivery.Id &&
                d.Team!.Id == team.Id &&
                d.Fields!.Any(f =>
                    f.AssignmentFieldId == field.AssignmentFieldId &&
                    f.JsonValue == jsonValue
                )
            ));
        }
    }

    [Fact]
    public async Task UpdateDelivery_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);

        var textField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.ShortText);
        var intField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Integer);
        var floatField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Float);
        var boolField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Boolean);

        var delivery = ModelFactory.CreateStudentDeliveryWithFields(assignment.Id, [textField, intField, floatField, boolField], student.Id);

        await DbContext.SaveChangesAsync();

        var request = new UpdateDeliveryRequest
        {
            Fields = [
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = textField.Id,
                    Value = false
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = intField.Id,
                    Value = 5.7
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = floatField.Id,
                    Value = "Value"
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = boolField.Id,
                    Value = 16
                }
            ]
        };

        var response = await Client.PutAsJsonAsync($"deliveries/{delivery.Id}", request);

        await Verify(response);
    }

    [Fact]
    public async Task UpdateDelivery_ShouldReturnBadRequest_WhenDueDatePassed()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, offset: TimeSpan.FromDays(-10_000));
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var request = new UpdateDeliveryRequest
        {
            Fields = []
        };

        var response = await Client.PutAsJsonAsync($"deliveries/{delivery.Id}", request);

        await Verify(response);
    }

    [Fact]
    public async Task UpdateDelivery_ShouldReturnNotFound_WhenInvalidDelivery()
    {
        var request = new UpdateDeliveryRequest
        {
            Fields = []
        };
        var deliveryId = Guid.NewGuid();

        var response = await Client.PutAsJsonAsync($"deliveries/{deliveryId}", request);

        await Verify(response);
    }
}