namespace Api.Tests.Integration.Teams;

public class GetTeamByStudentCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetTeamByStudentCourse_ShouldReturnTeam_WhenTeamExists()
    {
        var course = ModelFactory.CreateCourse();
        var students = ModelFactory.CreateCourseStudents(course.Id, 3);
        ModelFactory.CreateTeam(course.Id, 1, students);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{students[0].Id}/courses/{course.Id}/teams");

        await Verify(response);
    }

    [Fact]
    public async Task GetTeamByStudentCourse_ShouldReturnNotFound_WhenNoTeamExists()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/courses/{course.Id}/teams");

        await Verify(response);
    }

    [Fact]
    public async Task GetTeamByStudentCourse_ShouldReturnBadRequest_WhenStudentNotInCourse()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/courses/{course.Id}/teams");

        await Verify(response);
    }

    [Fact]
    public async Task GetTeamByStudentCourse_ShouldReturnNotFound_WhenInvalidStudent()
    {
        var course = ModelFactory.CreateCourse();
        var studentId = Guid.NewGuid();
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{studentId}/courses/{course.Id}/teams");

        await Verify(response);
    }

    [Fact]
    public async Task GetTeamByStudentCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var student = ModelFactory.CreateStudent();
        var courseId = Guid.NewGuid();
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/courses/{courseId}/teams");

        await Verify(response);
    }
}