namespace Api.Tests.Integration.Courses;

public class GetAllCoursesTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllCourses_ShouldReturnEmpty_WhenEmpty()
    {
        var response = await Client.GetAsync("courses");
        await Verify(response);
    }

    [Fact]
    public async Task GetAllCourses_ShouldReturnCourses_WhenCoursesExists()
    {
        ModelFactory.CreateCourses(3);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync("courses");
        await Verify(response);
    }
}
