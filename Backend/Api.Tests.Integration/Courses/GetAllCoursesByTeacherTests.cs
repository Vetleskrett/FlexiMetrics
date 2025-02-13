namespace Api.Tests.Integration.Courses;

public class GetAllCoursesByTeacherTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllCoursesByTeacher_ShouldReturnEmpty_WhenEmpty()
    {
        ModelFactory.CreateCourses(3);
        var teacher = ModelFactory.CreateTeacher();
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teachers/{teacher.Id}/courses");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllCoursesByTeacher_ShouldReturnCourses_WhenCoursesExists()
    {
        var courses = ModelFactory.CreateCourses(5);
        var teacher = ModelFactory.CreateTeacher();
        ModelFactory.CreateCourseTeacher(courses[0].Id, teacher.Id);
        ModelFactory.CreateCourseTeacher(courses[1].Id, teacher.Id);
        ModelFactory.CreateCourseTeacher(courses[2].Id, teacher.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teachers/{teacher.Id}/courses");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllCoursesByTeacher_ShouldReturnNotFound_WhenInvalidTeacher()
    {
        var teacherId = Guid.NewGuid();

        var response = await Client.GetAsync($"teachers/{teacherId}/courses");

        await Verify(response);
    }
}