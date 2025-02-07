namespace Api.Tests.Integration.AssignmentFields;

public class GetAllAssignmentFieldsByAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllAssignmentFieldsByAssignment_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var assignment = ModelFactory.GetValidAssignment(course.Id);
        var otherAssignment = ModelFactory.GetValidAssignment(course.Id);
        DbContext.Assignments.Add(assignment);
        DbContext.Assignments.Add(otherAssignment);

        DbContext.AssignmentFields.AddRange([
            ModelFactory.GetValidAssignmentField(otherAssignment.Id),
            ModelFactory.GetValidAssignmentField(otherAssignment.Id),
            ModelFactory.GetValidAssignmentField(otherAssignment.Id)
        ]);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"assignments/{assignment.Id}/fields");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentFieldsByAssignment_ShouldReturnAssignmentFields_WhenAssignmentFieldsExists()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var assignment = ModelFactory.GetValidAssignment(course.Id);
        var otherAssignment = ModelFactory.GetValidAssignment(course.Id);
        DbContext.Assignments.Add(assignment);
        DbContext.Assignments.Add(otherAssignment);

        DbContext.AssignmentFields.AddRange([
            ModelFactory.GetValidAssignmentField(assignment.Id),
            ModelFactory.GetValidAssignmentField(assignment.Id),
            ModelFactory.GetValidAssignmentField(assignment.Id),

            ModelFactory.GetValidAssignmentField(otherAssignment.Id),
            ModelFactory.GetValidAssignmentField(otherAssignment.Id)
        ]);
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