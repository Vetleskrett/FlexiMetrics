using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Students;

public class RemoveStudentFromCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task RemoveStudentFromCourse_ShouldRemoveStudentFromCourse_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        User student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var response = Client.DeleteAsync($"courses/{course.Id}/students/{student.Id}");

        await Verify(response);
        Assert.False(await DbContext.CourseStudents.AnyAsync(ct => ct.StudentId == student.Id));
    }

    [Fact]
    public async Task RemoveStudentFromCourse_ShouldReturnBadRequest_WhenNotInCourse()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        await DbContext.SaveChangesAsync();

        var response = Client.DeleteAsync($"courses/{course.Id}/students/{student.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task RemoveStudentFromCourse_ShouldReturnNotFound_WhenInvalidStudent()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();
        var studentId = Guid.NewGuid();

        var response = Client.DeleteAsync($"courses/{course.Id}/students/{studentId}");

        await Verify(response);
    }

    [Fact]
    public async Task RemoveStudentFromCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var student = ModelFactory.CreateStudent();
        await DbContext.SaveChangesAsync();
        var courseId = Guid.NewGuid();

        var response = Client.DeleteAsync($"courses/{courseId}/students/{student.Id}");

        await Verify(response);
    }
}