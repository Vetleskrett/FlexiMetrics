namespace Api.Tests.Integration.Teams;

public class GetTeamTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetTeam_ShouldReturnEmptyStudents_WhenNoStudents()
    {
        var course = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(course.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teams/{team.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetTeam_ShouldReturnTeam_WhenTeamExists()
    {
        var course = ModelFactory.CreateCourse();
        var students = ModelFactory.CreateCourseStudents(course.Id, 3);
        var team = ModelFactory.CreateTeam(course.Id, 1, students);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teams/{team.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetTeam_ShouldReturnNotFound_WhenInvalidTeam()
    {
        var id = Guid.NewGuid();

        var response = await Client.GetAsync($"teams/{id}");

        await Verify(response);
    }
}