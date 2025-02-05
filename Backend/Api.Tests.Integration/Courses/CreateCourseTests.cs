using Api.Courses.Contracts;
using Database.Models;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Courses;

public class CreateCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateCourse_ShouldCreateCourse_WhenValidRequest()
    {
        var request = new CreateCourseRequest
        {
            Code = "TDT1001",
            Name = "Webutvikling",
            Year = 2025,
            Semester = Semester.Spring
        };

        var response = await Client.PostAsJsonAsync($"courses", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateCourse_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var request = new CreateCourseRequest
        {
            Code = "",
            Name = "",
            Year = 0,
            Semester = (Semester)100
        };

        var response = await Client.PostAsJsonAsync($"courses", request);

        await Verify(response);
    }
}