namespace Api.Tests.Integration.Assignments;

public class GetAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAssignment_ShouldReturnAssignment_WhenAssignmentExists()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var assignment = ModelFactory.GetValidAssignment(course.Id);
        DbContext.Assignments.Add(assignment);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"assignments/{assignment.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetAssignment_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var id = Guid.NewGuid();

        var response = await Client.GetAsync($"assignments/{id}");

        await Verify(response);
    }
}