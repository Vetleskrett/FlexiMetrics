using Database.Models;

namespace Api.Tests.Integration.Deliveries;

public class GetAllDeliveriesTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllDeliveries_ShouldReturnEmpty_WhenEmpty()
    {
        var response = await Client.GetAsync("deliveries");
        await Verify(response);
    }

    [Fact]
    public async Task GetAllDeliveries_ShouldReturnEmptyFields_WhenNoFields()
    {
        var course = ModelFactory.CreateCourse();

        var studentAssignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);

        var teamAssignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);

        var students = ModelFactory.CreateCourseStudents(course.Id, 4);
        var teams = ModelFactory.CreateTeamsWithStudents(course.Id, students, 2);

        ModelFactory.CreateStudentDeliveries(studentAssignment.Id, students);
        ModelFactory.CreateTeamDeliveries(teamAssignment.Id, teams);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync("deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllDeliveries_ShouldReturnDeliveries_WhenDeliveriesExist()
    {
        var course = ModelFactory.CreateCourse();

        var studentAssignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var studentAssignmentFields = ModelFactory.CreateAssignmentFields(studentAssignment.Id, 3);

        var teamAssignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);
        var teamAssignmentFields = ModelFactory.CreateAssignmentFields(teamAssignment.Id, 3);

        var students = ModelFactory.CreateCourseStudents(course.Id, 4);
        var teams = ModelFactory.CreateTeamsWithStudents(course.Id, students, 2);

        ModelFactory.CreateStudentDeliveriesWithFields(studentAssignment.Id, studentAssignmentFields, students);
        ModelFactory.CreateTeamDeliveriesWithFields(teamAssignment.Id, teamAssignmentFields, teams);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync("deliveries");

        await Verify(response);
    }
}