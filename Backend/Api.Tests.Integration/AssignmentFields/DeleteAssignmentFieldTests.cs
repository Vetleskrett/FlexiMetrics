using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.AssignmentFields;

public class DeleteAssignmentFieldTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteAssignmentField_ShouldDeleteAssignmentField_WhenValidAssignmentField()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var assignment = ModelFactory.GetValidAssignment(course.Id);
        DbContext.Assignments.Add(assignment);

        var field = ModelFactory.GetValidAssignmentField(assignment.Id);
        DbContext.AssignmentFields.Add(field);

        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"assignment-fields/{field.Id}");

        await Verify(response);
        Assert.False(await DbContext.AssignmentFields.AnyAsync());
    }

    [Fact]
    public async Task DeleteAssignmentField_ShouldDeleteDeliveryField_WhenValidAssignmentField()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var assignment = ModelFactory.GetValidAssignment(course.Id);
        DbContext.Assignments.Add(assignment);

        var field = ModelFactory.GetValidAssignmentField(assignment.Id);
        DbContext.AssignmentFields.Add(field);

        List<User> students = [
            ModelFactory.GetValidStudent("student1@ntnu.no"),
            ModelFactory.GetValidStudent("student2@ntnu.no"),
            ModelFactory.GetValidStudent("student3@ntnu.no")
        ];
        DbContext.Users.AddRange(students);

        DbContext.CourseStudents.AddRange([
            ModelFactory.GetValidCourseStudent(course.Id, students[0].Id),
            ModelFactory.GetValidCourseStudent(course.Id, students[1].Id),
            ModelFactory.GetValidCourseStudent(course.Id, students[2].Id),
        ]);

        List<Delivery> deliveries = [
            ModelFactory.GetValidStudentDelivery(assignment.Id, students[0].Id),
            ModelFactory.GetValidStudentDelivery(assignment.Id, students[1].Id),
            ModelFactory.GetValidStudentDelivery(assignment.Id, students[2].Id),
        ];
        DbContext.Deliveries.AddRange(deliveries);

        DbContext.DeliveryFields.AddRange([
            ModelFactory.GetValidDeliveryField(deliveries[0].Id, field),
            ModelFactory.GetValidDeliveryField(deliveries[1].Id, field),
            ModelFactory.GetValidDeliveryField(deliveries[2].Id, field)
        ]);

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