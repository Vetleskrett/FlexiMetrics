namespace Api.Tests.Integration.Courses;

public class GetCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetCourse_ShouldReturnCourse_WhenCourseExists()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"courses/{course.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var id = Guid.NewGuid();

        var response = await Client.GetAsync($"courses/{id}");

        await Verify(response);
    }
}
