namespace Api.Tests.Integration.Assignments;

public class GetAllAssignmentsTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllAssignments_ShouldReturnEmpty_WhenEmpty()
    {
        var response = await Client.GetAsync("assignments");
        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignments_ShouldReturnAssignments_WhenAssignmentsExists()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        DbContext.Assignments.AddRange([
            ModelFactory.GetValidAssignment(course.Id),
            ModelFactory.GetValidAssignment(course.Id),
            ModelFactory.GetValidAssignment(course.Id)
        ]);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync("assignments");
        await Verify(response);
    }
}