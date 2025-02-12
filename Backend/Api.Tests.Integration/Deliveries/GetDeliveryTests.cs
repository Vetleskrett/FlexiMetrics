using Database.Models;

namespace Api.Tests.Integration.Deliveries;

public class GetDeliveryTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetDelivery_ShouldReturnEmptyFields_WhenNoFields()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"deliveries/{delivery.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetDelivery_ShouldReturnStudentDelivery_WhenStudentDeliveryExists()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var assignmentFields = ModelFactory.CreateAssignmentFields(assignment.Id, 3);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var delivery = ModelFactory.CreateStudentDeliveryWithFields(assignment.Id, assignmentFields, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"deliveries/{delivery.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetDelivery_ShouldReturnTeamDelivery_WhenTeamDeliveryExists()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);
        var assignmentFields = ModelFactory.CreateAssignmentFields(assignment.Id, 3);
        var students = ModelFactory.CreateCourseStudents(course.Id, 2);
        var team = ModelFactory.CreateTeam(course.Id, students: students);
        var delivery = ModelFactory.CreateTeamDeliveryWithFields(assignment.Id, assignmentFields, team.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"deliveries/{delivery.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetDelivery_ShouldReturnNotFound_WhenInvalidDelivery()
    {
        var deliveryId = Guid.NewGuid();

        var response = await Client.GetAsync($"deliveries/{deliveryId}");

        await Verify(response);
    }
}