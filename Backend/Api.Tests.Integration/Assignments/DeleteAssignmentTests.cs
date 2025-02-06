using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Assignments;

public class DeleteAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteAssignment_ShouldDeleteAssignment_WhenValidAssignment()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var assignment = ModelFactory.GetValidAssignment(course.Id);
        DbContext.Assignments.Add(assignment);

        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"assignments/{assignment.Id}");

        await Verify(response);
        Assert.False(await DbContext.Assignments.AnyAsync());
    }

    [Fact]
    public async Task DeleteAssignment_ShouldDeleteAssignmentFields_WhenValidAssignment()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var assignment = ModelFactory.GetValidAssignment(course.Id);
        DbContext.Assignments.Add(assignment);

        DbContext.AssignmentFields.AddRange([
            ModelFactory.GetValidAssignmentField(assignment.Id),
            ModelFactory.GetValidAssignmentField(assignment.Id),
            ModelFactory.GetValidAssignmentField(assignment.Id),
        ]);

        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"assignments/{assignment.Id}");

        Assert.False(await DbContext.AssignmentFields.AnyAsync());
    }

    [Fact]
    public async Task DeleteAssignment_ShouldDeleteDeliveries_WhenValidAssignment()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var assignment = ModelFactory.GetValidAssignment(course.Id);
        DbContext.Assignments.Add(assignment);

        List<User> students = [
            ModelFactory.GetValidStudent("student1@ntnu.no"),
            ModelFactory.GetValidStudent("student2@ntnu.no"),
            ModelFactory.GetValidStudent("student3@ntnu.no"),
        ];
        DbContext.Users.AddRange(students);

        DbContext.CourseStudents.AddRange([
            ModelFactory.GetValidCourseStudent(course.Id, students[0].Id),
            ModelFactory.GetValidCourseStudent(course.Id, students[1].Id),
            ModelFactory.GetValidCourseStudent(course.Id, students[2].Id),
        ]);

        DbContext.Deliveries.AddRange([
            ModelFactory.GetValidStudentDelivery(assignment.Id, students[0].Id),
            ModelFactory.GetValidStudentDelivery(assignment.Id, students[1].Id),
            ModelFactory.GetValidStudentDelivery(assignment.Id, students[2].Id),
        ]);

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