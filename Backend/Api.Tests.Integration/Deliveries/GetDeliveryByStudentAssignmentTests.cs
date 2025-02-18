using Database.Models;

namespace Api.Tests.Integration.Deliveries;

public class GetDeliveryByStudentAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetDeliveryByStudentAssignment_ShouldReturnStudentDelivery_WhenStudentDeliveryExists()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var assignmentFields = ModelFactory.CreateAssignmentFields(assignment.Id, 3);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        ModelFactory.CreateStudentDeliveryWithFields(assignment.Id, assignmentFields, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetDeliveryByStudentAssignment_ShouldReturnTeamDelivery_WhenTeamDeliveryExists()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);
        var assignmentFields = ModelFactory.CreateAssignmentFields(assignment.Id, 3);
        var students = ModelFactory.CreateCourseStudents(course.Id, 2);
        var team = ModelFactory.CreateTeam(course.Id, students: students);
        ModelFactory.CreateTeamDeliveryWithFields(assignment.Id, assignmentFields, team.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{students[0].Id}/assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetDeliveryByStudentAssignment_ShouldReturnNoContent_WhenNoDeliveryExists()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetDeliveryByStudentAssignment_ShouldReturnBadRequest_WhenStudentNotInCourse()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        var student = ModelFactory.CreateStudent();
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetDeliveryByStudentAssignment_ShouldReturnNotFound_WhenInvalidStudent()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        await DbContext.SaveChangesAsync();
        var studentId = Guid.NewGuid();

        var response = await Client.GetAsync($"students/{studentId}/assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetDeliveryByStudentAssignment_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var student = ModelFactory.CreateStudent();
        await DbContext.SaveChangesAsync();
        var assignmentId = Guid.NewGuid();

        var response = await Client.GetAsync($"students/{student.Id}/assignments/{assignmentId}/deliveries");

        await Verify(response);
    }
}