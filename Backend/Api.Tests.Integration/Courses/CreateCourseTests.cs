using Api.Courses.Contracts;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Courses;

public class CreateCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateCourse_ShouldCreateCourse_WhenValidRequest()
    {
        var teacher = ModelFactory.GetValidTeacher();
        DbContext.Users.Add(teacher);
        await DbContext.SaveChangesAsync();

        var request = new CreateCourseRequest
        {
            Code = "TDT1001",
            Name = "Webutvikling",
            Year = 2025,
            Semester = Semester.Spring,
            TeacherId = teacher.Id,
        };

        var response = await Client.PostAsJsonAsync($"courses", request);

        await Verify(response);
        Assert.NotNull(await DbContext.CourseTeachers.FirstOrDefaultAsync(x => x.TeacherId == teacher.Id));
    }

    [Fact]
    public async Task CreateCourse_ShouldReturnNotFound_WhenInvalidTeacher()
    {
        var request = new CreateCourseRequest
        {
            Code = "TDT1001",
            Name = "Webutvikling",
            Year = 2025,
            Semester = Semester.Spring,
            TeacherId = Guid.NewGuid(),
        };

        var response = await Client.PostAsJsonAsync($"courses", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateCourse_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var teacher = ModelFactory.GetValidTeacher();
        DbContext.Users.Add(teacher);
        await DbContext.SaveChangesAsync();

        var request = new CreateCourseRequest
        {
            Code = "",
            Name = "",
            Year = 0,
            Semester = (Semester)100,
            TeacherId = teacher.Id,
        };

        var response = await Client.PostAsJsonAsync($"courses", request);

        await Verify(response);
    }
}