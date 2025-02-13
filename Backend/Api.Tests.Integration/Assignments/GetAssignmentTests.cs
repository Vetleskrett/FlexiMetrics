namespace Api.Tests.Integration.Assignments;

public class GetAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAssignment_ShouldReturnAssignment_WhenAssignmentExists()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
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