namespace Api.Tests.Integration.Teams;

public class GetAllTeamsByCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllTeamsByCourse_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.CreateCourse();
        var otherCourse = ModelFactory.CreateCourse();
        ModelFactory.CreateTeams(otherCourse.Id, 3);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"courses/{course.Id}/teams");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllTeamsByCourse_ShouldReturnEmptyStudents_WhenNoStudents()
    {
        var course = ModelFactory.CreateCourse();
        ModelFactory.CreateTeams(course.Id, 3);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"courses/{course.Id}/teams");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllTeamsByCourse_ShouldReturnTeams_WhenTeamsExists()
    {
        var courses = ModelFactory.CreateCourses(2);
        var students = ModelFactory.CreateStudents(6);
        foreach (var course in courses)
        {
            foreach (var student in students)
            {
                ModelFactory.CreateCourseStudent(course.Id, student.Id);
            }
        }
        ModelFactory.CreateTeamsWithStudents(courses[0].Id, students, 3);
        ModelFactory.CreateTeamsWithStudents(courses[1].Id, students, 2);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"courses/{courses[0].Id}/teams");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllTeamsByCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var courseId = Guid.NewGuid();

        var response = await Client.GetAsync($"courses/{courseId}/teams");

        await Verify(response);
    }
}