namespace Api.Tests.Integration.Courses;

public class GetAllCoursesByStudentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllCoursesByStudent_ShouldReturnEmpty_WhenEmpty()
    {
        ModelFactory.CreateCourses(3);
        var student = ModelFactory.CreateStudent();
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/courses");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllCoursesByStudent_ShouldReturnCourses_WhenCoursesExists()
    {
        var courses = ModelFactory.CreateCourses(5);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(courses[0].Id, student.Id);
        ModelFactory.CreateCourseStudent(courses[1].Id, student.Id);
        ModelFactory.CreateCourseStudent(courses[2].Id, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/courses");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllCoursesByStudent_ShouldReturnNotFound_WhenInvalidStudent()
    {
        var studentId = Guid.NewGuid();

        var response = await Client.GetAsync($"students/{studentId}/courses");

        await Verify(response);
    }
}