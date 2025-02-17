namespace Api.Tests.Integration.Feedbacks;

public class GetAllFeedbacksByAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllFeedbacksByAssignment_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.CreateCourse();
        var students = ModelFactory.CreateCourseStudents(course.Id, 3);
        var assignment = ModelFactory.CreateAssignment(course.Id);
        var otherAssignment = ModelFactory.CreateAssignment(course.Id);
        ModelFactory.CreateFeedbacks(otherAssignment.Id, students);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"assignments/{assignment.Id}/feedbacks");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllFeedbacksByAssignment_ShouldReturnFeedbacks_WhenFeedbacksExists()
    {
        var course = ModelFactory.CreateCourse();
        var students = ModelFactory.CreateCourseStudents(course.Id, 3);
        var assignment = ModelFactory.CreateAssignment(course.Id);
        var otherAssignment = ModelFactory.CreateAssignment(course.Id);
        ModelFactory.CreateFeedbacks(assignment.Id, students);
        ModelFactory.CreateFeedbacks(otherAssignment.Id, students);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"assignments/{assignment.Id}/feedbacks");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllFeedbacksByAssignment_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var assignmentId = Guid.NewGuid();

        var response = await Client.GetAsync($"assignments/{assignmentId}/feedbacks");

        await Verify(response);
    }
}
