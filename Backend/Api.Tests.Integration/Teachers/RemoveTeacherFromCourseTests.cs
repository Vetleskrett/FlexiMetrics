using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Teachers;

public class RemoveTeacherFromCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task RemoveTeacherFromCourse_ShouldRemoveTeacherFromCourse_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var teachers = ModelFactory.CreateCourseTeachers(course.Id, 3);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"courses/{course.Id}/teachers/{teachers[0].Id}");

        await Verify(response);
        Assert.False(await DbContext.CourseTeachers.AnyAsync(ct => ct.TeacherId == teachers[0].Id));
    }

    [Fact]
    public async Task RemoveTeacherFromCourse_ShouldReturnBadRequest_WhenOnlyOneTeacher()
    {
        var course = ModelFactory.CreateCourse();
        var teacher = ModelFactory.CreateTeacher();
        ModelFactory.CreateCourseTeacher(course.Id, teacher.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"courses/{course.Id}/teachers/{teacher.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task RemoveTeacherFromCourse_ShouldReturnBadRequest_WhenNotInCourse()
    {
        var course = ModelFactory.CreateCourse();
        var teacher = ModelFactory.CreateTeacher();
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"courses/{course.Id}/teachers/{teacher.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task RemoveTeacherFromCourse_ShouldReturnNotFound_WhenInvalidTeacher()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();
        var teacherId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"courses/{course.Id}/teachers/{teacherId}");

        await Verify(response);
    }

    [Fact]
    public async Task RemoveTeacherFromCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var teacher = ModelFactory.CreateTeacher();
        await DbContext.SaveChangesAsync();
        var courseId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"courses/{courseId}/teachers/{teacher.Id}");

        await Verify(response);
    }
}