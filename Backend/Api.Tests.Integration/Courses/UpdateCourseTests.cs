using Api.Courses.Contracts;
using Database.Models;
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
        var id = Guid.NewGuid();

        var request = new UpdateCourseRequest
        {
            Code = "TDT9999",
            Name = "Databaser",
            Year = 2028,
            Semester = Semester.Autumn
        };

        var response = await Client.PutAsJsonAsync($"courses/{id}", request);

        await Verify(response);
    }
}