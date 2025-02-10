using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Assignments;

public class DeleteAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteAssignment_ShouldDeleteAssignment_WhenValidAssignment()
    {
        var course = ModelFactory.CreateCourse();

        var assignment = ModelFactory.CreateAssignment(course.Id);

        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"assignments/{assignment.Id}");

        await Verify(response);
        Assert.False(await DbContext.Assignments.AnyAsync());
    }

    [Fact]
    public async Task DeleteAssignment_ShouldDeleteAssignmentFields_WhenValidAssignment()
    {
        var course = ModelFactory.CreateCourse();

        var assignment = ModelFactory.CreateAssignment(course.Id);

        ModelFactory.CreateAssignmentFields(assignment.Id, 3);

        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"assignments/{assignment.Id}");

        Assert.False(await DbContext.AssignmentFields.AnyAsync());
    }

    [Fact]
    public async Task DeleteAssignment_ShouldDeleteDeliveries_WhenValidAssignment()
    {
        var course = ModelFactory.CreateCourse();

        var assignment = ModelFactory.CreateAssignment(course.Id);

        var students = ModelFactory.CreateCourseStudents(course.Id, 3);

        ModelFactory.CreateStudentDeliveries(assignment.Id, students);

        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"assignments/{assignment.Id}");

        Assert.False(await DbContext.Deliveries.AnyAsync());
    }

    [Fact]
    public async Task DeleteAssignment_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var id = Guid.NewGuid();

        var response = await Client.DeleteAsync($"assignments/{id}");

        await Verify(response);
    }
}