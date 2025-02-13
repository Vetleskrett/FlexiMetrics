using Api.Courses.Contracts;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Courses;

public class UpdateCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateCourse_ShouldUpdateCourse_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();

        var request = new UpdateCourseRequest
        {
            Code = "TDT9999",
            Name = "Databaser",
            Year = 2028,
            Semester = Semester.Autumn
        };

        var response = await Client.PutAsJsonAsync($"courses/{course.Id}", request);

        await Verify(response);
        Assert.True(await DbContext.Courses.AnyAsync(c =>
            c.Code == request.Code &&
            c.Name == request.Name &&
            c.Year == request.Year &&
            c.Semester == request.Semester
        ));
    }

    [Fact]
    public async Task UpdateCourse_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();

        var request = new UpdateCourseRequest
        {
            Code = "",
            Name = "",
            Year = 0,
            Semester = (Semester)100
        };

        var response = await Client.PutAsJsonAsync($"courses/{course.Id}", request);

        await Verify(response);
    }

    [Fact]
    public async Task UpdateCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var request = new UpdateCourseRequest
        {
            Code = "TDT9999",
            Name = "Databaser",
            Year = 2028,
            Semester = Semester.Autumn
        };
        var id = Guid.NewGuid();

        var response = await Client.PutAsJsonAsync($"courses/{id}", request);

        await Verify(response);
    }
}