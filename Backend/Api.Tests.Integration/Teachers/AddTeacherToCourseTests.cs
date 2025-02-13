using Api.Teachers.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Teachers;

public class AddTeacherToCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task AddTeacherToCourse_ShouldAddTeacherToCourse_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var teacher = ModelFactory.CreateTeacher();
        await DbContext.SaveChangesAsync();

        var request = new AddTeacherRequest
        {
            Email = teacher.Email
        };

        var response = Client.PostAsJsonAsync($"courses/{course.Id}/teachers", request);

        await Verify(response);
        Assert.True(await DbContext.CourseTeachers.AnyAsync(ct =>
            ct.CourseId == course.Id &&
            ct.TeacherId == teacher.Id
        ));
    }

    [Fact]
    public async Task AddTeacherToCourse_ShouldReturnBadRequest_WhenAlreadyInCourse()
    {
        var course = ModelFactory.CreateCourse();
        var teacher = ModelFactory.CreateTeacher();
        ModelFactory.CreateCourseTeacher(course.Id, teacher.Id);
        await DbContext.SaveChangesAsync();

        var request = new AddTeacherRequest
        {
            Email = teacher.Email
        };

        var response = Client.PostAsJsonAsync($"courses/{course.Id}/teachers", request);

        await Verify(response);
    }

    [Fact]
    public async Task AddTeacherToCourse_ShouldReturnNotFound_WhenInvalidTeacher()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();

        var request = new AddTeacherRequest
        {
            Email = "teacher@ntnu.no"
        };

        var response = Client.PostAsJsonAsync($"courses/{course.Id}/teachers", request);

        await Verify(response);
    }

    [Fact]
    public async Task AddTeacherToCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var teacher = ModelFactory.CreateTeacher();
        await DbContext.SaveChangesAsync();

        var request = new AddTeacherRequest
        {
            Email = teacher.Email
        };
        var courseId = Guid.NewGuid();

        var response = Client.PostAsJsonAsync($"courses/{courseId}/teachers", request);

        await Verify(response);
    }
}