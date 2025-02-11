using Database.Models;

namespace Api.Tests.Integration.Deliveries;

public class GetAllDeliveriesByAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllDeliveriesByAssignment_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllDeliveriesByAssignment_ShouldReturnStudentDeliveries_WhenStudentDeliveriesExist()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var assignmentFields = ModelFactory.CreateAssignmentFields(assignment.Id, 3);
        var students = ModelFactory.CreateCourseStudents(course.Id, 4);
        ModelFactory.CreateStudentDeliveriesWithFields(assignment.Id, assignmentFields, students);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllDeliveriesByAssignment_ShouldReturnTeamDeliveries_WhenTeamDeliveriesExist()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);
        var assignmentFields = ModelFactory.CreateAssignmentFields(assignment.Id, 3);
        var students = ModelFactory.CreateCourseStudents(course.Id, 4);
        var teams = ModelFactory.CreateTeamsWithStudents(course.Id, students, 2);
        ModelFactory.CreateTeamDeliveriesWithFields(assignment.Id, assignmentFields, teams);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllDeliveriesByAssignment_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var assignmentId = Guid.NewGuid();

        var response = await Client.GetAsync($"assignments/{assignmentId}/deliveries");

        await Verify(response);
    }
}