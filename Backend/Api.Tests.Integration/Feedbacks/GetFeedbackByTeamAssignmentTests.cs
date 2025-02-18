namespace Api.Tests.Integration.Feedbacks;

public class GetFeedbackByTeamAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetFeedbackByTeamAssignment_ShouldReturnFeedback_WhenFeedbackExists()
    {
        var course = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(course.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id);
        ModelFactory.CreateFeedback(assignment.Id, null, team.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teams/{team.Id}/assignments/{assignment.Id}/feedbacks");

        await Verify(response);
    }

    [Fact]
    public async Task GetFeedbackByTeamAssignment_ShouldReturnBadRequest_WhenTeamNotInCourse()
    {
        var course = ModelFactory.CreateCourse();
        var otherCourse = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(otherCourse.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teams/{team.Id}/assignments/{assignment.Id}/feedbacks");

        await Verify(response);
    }

    [Fact]
    public async Task GetFeedbackByTeamAssignment_ShouldReturnNoContent_WhenNoFeedbackExists()
    {
        var course = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(course.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teams/{team.Id}/assignments/{assignment.Id}/feedbacks");

        await Verify(response);
    }

    [Fact]
    public async Task GetFeedbackByTeamAssignment_ShouldReturnNotFound_WhenInvalidTeam()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        await DbContext.SaveChangesAsync();
        var teamId = Guid.NewGuid();

        var response = await Client.GetAsync($"teams/{teamId}/assignments/{assignment.Id}/feedbacks");

        await Verify(response);
    }

    [Fact]
    public async Task GetFeedbackByTeamAssignment_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var course = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(course.Id);
        await DbContext.SaveChangesAsync();
        var assignmentId = Guid.NewGuid();

        var response = await Client.GetAsync($"teams/{team.Id}/assignments/{assignmentId}/feedbacks");

        await Verify(response);
    }
}
