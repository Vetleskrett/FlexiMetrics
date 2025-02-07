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
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var assignment = ModelFactory.GetValidAssignment(course.Id);
        DbContext.Assignments.Add(assignment);

        DbContext.AssignmentFields.AddRange([
            ModelFactory.GetValidAssignmentField(assignment.Id),
            ModelFactory.GetValidAssignmentField(assignment.Id),
            ModelFactory.GetValidAssignmentField(assignment.Id)
        ]);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync("assignment-fields");

        await Verify(response);
    }
}