using Database.Models;

namespace Api.Tests.Integration.Students;

public class GetAllStudentsByCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllStudentsByCourse_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();

        var response = Client.GetAsync($"courses/{course.Id}/students");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllStudentsByCourse_ShouldReturnStudents_WhenStudentsExists()
    {
        var course = ModelFactory.CreateCourse();

        ModelFactory.CreateCourseStudents(course.Id, 3);

        await DbContext.SaveChangesAsync();

        var response = Client.GetAsync($"courses/{course.Id}/students");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllStudentsByCourse_ShouldNotFound_WhenInvalidCourse()
    {
        var id = Guid.NewGuid();

        var response = Client.GetAsync($"courses/{id}/students");

        await Verify(response);
    }
}