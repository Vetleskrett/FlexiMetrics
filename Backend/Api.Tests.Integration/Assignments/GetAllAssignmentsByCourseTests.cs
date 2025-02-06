namespace Api.Tests.Integration.Assignments;

public class GetAllAssignmentsByCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllAssignmentsByCourse_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.GetValidCourse();
        var otherCourse = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);
        DbContext.Courses.Add(otherCourse);

        DbContext.Assignments.AddRange([
            ModelFactory.GetValidAssignment(otherCourse.Id),
            ModelFactory.GetValidAssignment(otherCourse.Id),
            ModelFactory.GetValidAssignment(otherCourse.Id)
        ]);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByCourse_ShouldReturnAssignments_WhenAssignmentsExists()
    {
        var course = ModelFactory.GetValidCourse();
        var otherCourse = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);
        DbContext.Courses.Add(otherCourse);

        DbContext.Assignments.AddRange([
            ModelFactory.GetValidAssignment(course.Id),
            ModelFactory.GetValidAssignment(course.Id),
            ModelFactory.GetValidAssignment(course.Id),

            ModelFactory.GetValidAssignment(otherCourse.Id),
            ModelFactory.GetValidAssignment(otherCourse.Id)
        ]);
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