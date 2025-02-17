using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Teams;

public class DeleteTeamTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteTeam_ShouldDeleteTeam_WhenValidTeam()
    {
        var course = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(course.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"teams/{team.Id}");

        await Verify(response);
        Assert.False(await DbContext.Teams.AnyAsync(t => t.Id == team.Id));
    }

    [Fact]
    public async Task DeleteTeam_ShouldDeleteTeamDeliveries_WhenValidTeam()
    {
        var course = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(course.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id);
        var delivery = ModelFactory.CreateTeamDelivery(assignment.Id, team.Id);
        await DbContext.SaveChangesAsync();

        await Client.DeleteAsync($"teams/{team.Id}");

        Assert.False(await DbContext.Deliveries.AnyAsync(d => d.Id == delivery.Id));
    }

    [Fact]
    public async Task DeleteAssignment_ShouldDeleteFeedback_WhenValidAssignment()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var team = ModelFactory.CreateTeam(course.Id);
        ModelFactory.CreateFeedback(assignment.Id, null, team.Id);
        await DbContext.SaveChangesAsync();

        await Client.DeleteAsync($"teams/{team.Id}");

        Assert.False(await DbContext.Feedbacks.AnyAsync());
    }

    [Fact]
    public async Task DeleteTeam_ShouldReturnNotFound_WhenInvalidTeam()
    {
        var teamId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"teams/{teamId}");

        await Verify(response);
    }
}