using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Teachers;

public class RemoveTeacherFromCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task RemoveTeacherFromCourse_ShouldRemoveTeacherFromCourse_WhenValidRequest()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        List<User> teachers = [
            ModelFactory.GetValidTeacher("teacher1@ntnu.no"),
            ModelFactory.GetValidTeacher("teacher2@ntnu.no"),
            ModelFactory.GetValidTeacher("teacher3@ntnu.no"),
        ];
        DbContext.Users.AddRange(teachers);

        DbContext.CourseTeachers.AddRange([
            ModelFactory.GetValidCourseTeacher(course.Id, teachers[0].Id),
            ModelFactory.GetValidCourseTeacher(course.Id, teachers[1].Id),
            ModelFactory.GetValidCourseTeacher(course.Id, teachers[2].Id),
        ]);

        await DbContext.SaveChangesAsync();

        var response = Client.DeleteAsync($"courses/{course.Id}/teachers/{teachers[0].Id}");

        await Verify(response);
        Assert.False(await DbContext.CourseTeachers.AnyAsync(ct => ct.TeacherId == teachers[0].Id));
    }

    [Fact]
    public async Task RemoveTeacherFromCourse_ShouldReturnBadRequest_WhenOnlyOneTeacher()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var teacher = ModelFactory.GetValidTeacher();
        DbContext.Users.Add(teacher);

        DbContext.CourseTeachers.Add(ModelFactory.GetValidCourseTeacher(course.Id, teacher.Id));

        await DbContext.SaveChangesAsync();

        var response = Client.DeleteAsync($"courses/{course.Id}/teachers/{teacher.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task RemoveTeacherFromCourse_ShouldReturnBadRequest_WhenNotInCourse()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        var teacher = ModelFactory.GetValidTeacher();
        DbContext.Users.Add(teacher);

        await DbContext.SaveChangesAsync();

        var response = Client.DeleteAsync($"courses/{course.Id}/teachers/{teacher.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task RemoveTeacherFromCourse_ShouldReturnNotFound_WhenInvalidTeacher()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        await DbContext.SaveChangesAsync();

        var teacherId = Guid.NewGuid();

        var response = Client.DeleteAsync($"courses/{course.Id}/teachers/{teacherId}");

        await Verify(response);
    }

    [Fact]
    public async Task RemoveTeacherFromCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var teacher = ModelFactory.GetValidTeacher();
        DbContext.Users.Add(teacher);

        await DbContext.SaveChangesAsync();

        var courseId = Guid.NewGuid();

        var response = Client.DeleteAsync($"courses/{courseId}/teachers/{teacher.Id}");

        await Verify(response);
    }
}