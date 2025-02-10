using Api.Teams.Contracts;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Teams;

public class CreateTeamsTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateTeams_ShouldCreateTeams_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        ModelFactory.CreateTeam(course.Id, 1);
        ModelFactory.CreateTeam(course.Id, 4);
        ModelFactory.CreateTeam(course.Id, 5);
        await DbContext.SaveChangesAsync();

        var request = new CreateTeamsRequest
        {
            CourseId = course.Id,
            NumTeams = 3
        };

        var response = await Client.PostAsJsonAsync("teams", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateTeams_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var request = new CreateTeamsRequest
        {
            CourseId = Guid.NewGuid(),
            NumTeams = 3
        };

        var response = await Client.PostAsJsonAsync("teams", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateTeams_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();

        var request = new CreateTeamsRequest
        {
            CourseId = course.Id,
            NumTeams = -1
        };

        var response = await Client.PostAsJsonAsync("teams", request);

        await Verify(response);
    }
}