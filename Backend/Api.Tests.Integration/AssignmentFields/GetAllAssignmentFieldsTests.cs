namespace Api.Tests.Integration.AssignmentFields;

public class GetAllAssignmentFieldsTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllAssignmentFields_ShouldReturnEmpty_WhenEmpty()
    {
        var response = await Client.GetAsync("assignment-fields");
        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentFields_ShouldReturnAssignmentFields_WhenAssignmentFieldsExists()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(course.Id);
        ModelFactory.CreateAssignmentFields(assignment.Id, 3);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync("assignment-fields");

        await Verify(response);
    }
}