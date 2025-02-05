using Database.Models;

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
        DbContext.AddRange([
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
        ]);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync("courses");
        await Verify(response);
    }
}
