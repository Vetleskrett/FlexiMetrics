using Api.Deliveries.Contracts;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.Tests.Integration.Deliveries;

public class CreateDeliveryTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateDelivery_ShouldCreateStudentDelivery_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);

        var stringField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.ShortText);
        var intField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Integer);
        var doubleField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Float);
        var boolField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Boolean);

        await DbContext.SaveChangesAsync();

        var request = new CreateDeliveryRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Fields = [
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = stringField.Id,
                    Value = "Value"
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = intField.Id,
                    Value = 16
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = doubleField.Id,
                    Value = 5.7
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = boolField.Id,
                    Value = false
                }
            ]
        };

        var response = await Client.PostAsJsonAsync("deliveries", request);

        await Verify(response);
        foreach (var field in request.Fields)
        {
            var jsonValue = JsonSerializer.SerializeToDocument(field.Value);
            Assert.True(await DbContext.Deliveries.AnyAsync(d =>
                d.AssignmentId == request.AssignmentId &&
                d.StudentId == request.StudentId &&
                d.Fields!.Any(f =>
                    f.AssignmentFieldId == field.AssignmentFieldId &&
                    f.JsonValue == jsonValue
                )
            ));
        }
    }

    [Fact]
    public async Task CreateDelivery_ShouldCreateTeamDelivery_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var students = ModelFactory.CreateCourseStudents(course.Id, 2);
        ModelFactory.CreateTeam(course.Id, students: students);
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);

        var stringField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.ShortText);
        var intField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Integer);
        var doubleField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Float);
        var boolField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Boolean);

        await DbContext.SaveChangesAsync();

        var request = new CreateDeliveryRequest
        {
            AssignmentId = assignment.Id,
            StudentId = students[0].Id,
            Fields = [
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = stringField.Id,
                    Value = "Value"
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = intField.Id,
                    Value = 16
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = doubleField.Id,
                    Value = 5.7
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = boolField.Id,
                    Value = false
                }
            ]
        };

        var response = await Client.PostAsJsonAsync("deliveries", request);

        await Verify(response);
        foreach (var field in request.Fields)
        {
            var jsonValue = JsonSerializer.SerializeToDocument(field.Value);
            Assert.True(await DbContext.Deliveries.AnyAsync(d =>
                d.AssignmentId == request.AssignmentId &&
                d.Team!.Students.Any(s => s.Id == request.StudentId) &&
                d.Fields!.Any(f =>
                    f.AssignmentFieldId == field.AssignmentFieldId &&
                    f.JsonValue == jsonValue
                )
            ));
        }
    }

    [Fact]
    public async Task CreateDelivery_ShouldDeleteExistingStudentDelivery_WhenDeliveryExists()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var existingDelivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var request = new CreateDeliveryRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Fields = []
        };

        var response = await Client.PostAsJsonAsync("deliveries", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.False(await DbContext.Deliveries.AnyAsync(d => d.Id == existingDelivery.Id));
    }

    [Fact]
    public async Task CreateDelivery_ShouldDeleteExistingTeamDelivery_WhenDeliveryExists()
    {
        var course = ModelFactory.CreateCourse();
        var students = ModelFactory.CreateCourseStudents(course.Id, 2);
        var team = ModelFactory.CreateTeam(course.Id, students: students);
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);
        var existingDelivery = ModelFactory.CreateTeamDelivery(assignment.Id, team.Id);
        await DbContext.SaveChangesAsync();

        var request = new CreateDeliveryRequest
        {
            AssignmentId = assignment.Id,
            StudentId = students[0].Id,
            Fields = []
        };

        var response = await Client.PostAsJsonAsync("deliveries", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.False(await DbContext.Deliveries.AnyAsync(d => d.Id == existingDelivery.Id));
    }

    [Fact]
    public async Task CreateDelivery_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var otherAssignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);

        var otherField = ModelFactory.CreateAssignmentField(otherAssignment.Id);
        ModelFactory.CreateAssignmentField(assignment.Id); // Missing field
        var repeatField = ModelFactory.CreateAssignmentField(assignment.Id);

        var textField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.ShortText);
        var intField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Integer);
        var floatField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Float);
        var boolField = ModelFactory.CreateAssignmentField(assignment.Id, AssignmentDataType.Boolean);

        await DbContext.SaveChangesAsync();

        var request = new CreateDeliveryRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Fields = [
               new DeliveryFieldRequest
                {
                    AssignmentFieldId = otherField.Id,
                    Value = "Value"
                },

                new DeliveryFieldRequest
                {
                    AssignmentFieldId = repeatField.Id,
                    Value = "Value"
                },
                new DeliveryFieldRequest
                {
                    AssignmentFieldId = repeatField.Id,
                    Value = "Value"
                },

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

        var response = await Client.PostAsJsonAsync("deliveries", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateDelivery_ShouldReturnBadRequest_WhenStudentNotInCourse()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        await DbContext.SaveChangesAsync();

        var request = new CreateDeliveryRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Fields = []
        };

        var response = await Client.PostAsJsonAsync("deliveries", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateDelivery_ShouldReturnBadRequest_WhenDueDatePassed()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, offset: TimeSpan.FromDays(-10_000));
        await DbContext.SaveChangesAsync();

        var request = new CreateDeliveryRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Fields = []
        };

        var response = await Client.PostAsJsonAsync("deliveries", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateDelivery_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        await DbContext.SaveChangesAsync();
        var assignmentId = Guid.NewGuid();

        var request = new CreateDeliveryRequest
        {
            AssignmentId = assignmentId,
            StudentId = student.Id,
            Fields = []
        };

        var response = await Client.PostAsJsonAsync("deliveries", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateDelivery_ShouldReturnNotFound_WhenInvalidStudent()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        await DbContext.SaveChangesAsync();
        var studentId = Guid.NewGuid();

        var request = new CreateDeliveryRequest
        {
            AssignmentId = assignment.Id,
            StudentId = studentId,
            Fields = []
        };

        var response = await Client.PostAsJsonAsync("deliveries", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateDelivery_ShouldReturnNotFound_WhenInvalidTeam()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);
        await DbContext.SaveChangesAsync();

        var request = new CreateDeliveryRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Fields = []
        };

        var response = await Client.PostAsJsonAsync("deliveries", request);

        await Verify(response);
    }
}