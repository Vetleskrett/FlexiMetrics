namespace Api.Tests.Integration.Assignments;

public class GetAllAssignmentsByCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllAssignmentsByCourse_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.CreateCourse();
        var otherCourse = ModelFactory.CreateCourse();

        ModelFactory.CreateAssignments(otherCourse.Id, 3);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByCourse_ShouldReturnAssignments_WhenAssignmentsExists()
    {
        var course = ModelFactory.CreateCourse();
        var otherCourse = ModelFactory.CreateCourse();

        ModelFactory.CreateAssignments(course.Id, 3);
        ModelFactory.CreateAssignments(otherCourse.Id, 2);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var courseId = Guid.NewGuid();

        var response = await Client.GetAsync($"course/{courseId}/assignments");

        await Verify(response);
    }
}