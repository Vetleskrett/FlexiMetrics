namespace Api.Tests.Integration.AssignmentFields;

public class GetAllAssignmentFieldsByAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllAssignmentFieldsByAssignment_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        var otherAssignment = ModelFactory.CreateAssignment(course.Id);
        ModelFactory.CreateAssignmentFields(otherAssignment.Id, 3);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"assignments/{assignment.Id}/fields");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentFieldsByAssignment_ShouldReturnAssignmentFields_WhenAssignmentFieldsExists()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        var otherAssignment = ModelFactory.CreateAssignment(course.Id);
        ModelFactory.CreateAssignmentFields(assignment.Id, 3);
        ModelFactory.CreateAssignmentFields(otherAssignment.Id, 2);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"assignments/{assignment.Id}/fields");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentFieldsByAssignment_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var assignmentId = Guid.NewGuid();

        var response = await Client.GetAsync($"assignments/{assignmentId}/fields");

        await Verify(response);
    }
}