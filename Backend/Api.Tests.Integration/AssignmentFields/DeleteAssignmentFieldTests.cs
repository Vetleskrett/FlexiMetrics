using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.AssignmentFields;

public class DeleteAssignmentFieldTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteAssignmentField_ShouldDeleteAssignmentField_WhenValidAssignmentField()
    {
        var course = ModelFactory.CreateCourse();

        var assignment = ModelFactory.CreateAssignment(course.Id);

        var field = ModelFactory.CreateAssignmentField(assignment.Id);

        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"assignment-fields/{field.Id}");

        await Verify(response);
        Assert.False(await DbContext.AssignmentFields.AnyAsync());
    }

    [Fact]
    public async Task DeleteAssignmentField_ShouldDeleteDeliveryField_WhenValidAssignmentField()
    {
        var course = ModelFactory.CreateCourse();

        var assignment = ModelFactory.CreateAssignment(course.Id);

        var field = ModelFactory.CreateAssignmentField(assignment.Id);

        var students = ModelFactory.CreateCourseStudents(course.Id, 3);

        ModelFactory.CreateStudentDeliveriesWithFields(assignment.Id, [field], students);

        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"assignment-fields/{field.Id}");

        Assert.False(await DbContext.DeliveryFields.AnyAsync());
    }

    [Fact]
    public async Task DeleteAssignmentField_ShouldReturnNotFound_WhenInvalidAssignmentField()
    {
        var id = Guid.NewGuid();

        var response = await Client.DeleteAsync($"assignment-fields/{id}");

        await Verify(response);
    }
}