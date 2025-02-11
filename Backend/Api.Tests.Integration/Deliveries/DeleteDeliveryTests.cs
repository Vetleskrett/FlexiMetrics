using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Deliveries;

public class DeleteDeliveryTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteDelivery_ShouldDeleteStudentDelivery_WhenValidStudentDelivery()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"deliveries/{delivery.Id}");

        await Verify(response);
        Assert.False(await DbContext.Deliveries.AnyAsync(d => d.Id == delivery.Id));
    }

    [Fact]
    public async Task DeleteDelivery_ShouldDeleteTeamDelivery_WhenValidTeamDelivery()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);
        var students = ModelFactory.CreateCourseStudents(course.Id, 2);
        var team = ModelFactory.CreateTeam(course.Id, students: students);
        var delivery = ModelFactory.CreateTeamDelivery(assignment.Id, team.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"deliveries/{delivery.Id}");

        await Verify(response);
        Assert.False(await DbContext.Deliveries.AnyAsync(d => d.Id == delivery.Id));
    }

    [Fact]
    public async Task DeleteDelivery_ShouldDeleteDeliveryFields_WhenValidDelivery()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var assignmentFields = ModelFactory.CreateAssignmentFields(assignment.Id, 3);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var delivery = ModelFactory.CreateStudentDeliveryWithFields(assignment.Id, assignmentFields, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"deliveries/{delivery.Id}");

        Assert.False(await DbContext.DeliveryFields.AnyAsync(f => f.DeliveryId == delivery.Id));
    }

    [Fact]
    public async Task DeleteDelivery_ShouldDeleteFeedback_WhenValidDelivery()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        ModelFactory.CreateFeedback(delivery.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"deliveries/{delivery.Id}");

        Assert.False(await DbContext.Feedbacks.AnyAsync(f => f.DeliveryId == delivery.Id));
    }

    [Fact]
    public async Task DeleteDelivery_ShouldReturnNotFound_WhenInvalidDelivery()
    {
        var deliveryId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"deliveries/{deliveryId}");

        await Verify(response);
    }
}