using Database.Models;

namespace Api.Tests.Integration.Deliveries;

public class GetDeliveryByTeamAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetDeliveryByTeamAssignment_ShouldReturnDelivery_WhenDeliveryExists()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);
        var assignmentFields = ModelFactory.CreateAssignmentFields(assignment.Id, 3);
        var team = ModelFactory.CreateTeam(course.Id);
        ModelFactory.CreateTeamDeliveryWithFields(assignment.Id, assignmentFields, team.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teams/{team.Id}/assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetDeliveryByTeamAssignment_ShouldReturnNoContent_WhenNoDeliveryExists()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);
        var team = ModelFactory.CreateTeam(course.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teams/{team.Id}/assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetDeliveryByTeamAssignment_ShouldReturnBadRequest_WhenAssignmentIsIndividual()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Individual);
        var team = ModelFactory.CreateTeam(course.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teams/{team.Id}/assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetDeliveryByTeamAssignment_ShouldReturnBadRequest_WhenTeamNotInCourse()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);
        var otherCourse = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(otherCourse.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teams/{team.Id}/assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetDeliveryByTeamAssignment_ShouldReturnNotFound_WhenInvalidTeam()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id, collaboration: CollaborationType.Teams);
        await DbContext.SaveChangesAsync();
        var teamId = Guid.NewGuid();

        var response = await Client.GetAsync($"teams/{teamId}/assignments/{assignment.Id}/deliveries");

        await Verify(response);
    }

    [Fact]
    public async Task GetDeliveryByTeamAssignment_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var course = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(course.Id); await DbContext.SaveChangesAsync();
        var assignmentId = Guid.NewGuid();

        var response = await Client.GetAsync($"teams/{team.Id}/assignments/{assignmentId}/deliveries");

        await Verify(response);
    }
}