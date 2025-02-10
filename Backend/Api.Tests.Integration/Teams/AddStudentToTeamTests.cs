using Api.Teams.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Teams;

public class AddStudentToTeamTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task AddStudentToTeam_ShouldAddStudentToTeam_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(course.Id);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var request = new AddStudentToTeamRequest
        {
            Email = student.Email
        };

        var response = Client.PostAsJsonAsync($"teams/{team.Id}/students", request);

        await Verify(response);
        Assert.True(await DbContext.Teams.AnyAsync(t => t.Id == team.Id && t.Students.Any(s => s.Id == student.Id)));
    }

    [Fact]
    public async Task AddStudentToTeam_ShouldCreateNewStudentAndAddToCourse_WhenStudentNotExists()
    {
        var course = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(course.Id);
        await DbContext.SaveChangesAsync();

        var request = new AddStudentToTeamRequest
        {
            Email = "new@ntnu.no"
        };

        var response = Client.PostAsJsonAsync($"teams/{team.Id}/students", request);

        Assert.True(await DbContext.Teams.AnyAsync(t => t.Id == team.Id && t.Students.Any(s => s.Email == "new@ntnu.no")));
        Assert.True(await DbContext.Users.AnyAsync(u => u.Email == "new@ntnu.no"));
        Assert.True(await DbContext.CourseStudents.AnyAsync(cs => cs.CourseId == course.Id && cs.Student!.Email == "new@ntnu.no"));
    }

    [Fact]
    public async Task AddStudentToTeam_ShouldReturnBadRequest_WhenAlreadyInTeam()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var team = ModelFactory.CreateTeam(course.Id, 1, [student]);
        await DbContext.SaveChangesAsync();

        var request = new AddStudentToTeamRequest
        {
            Email = student.Email
        };

        var response = Client.PostAsJsonAsync($"teams/{team.Id}/students", request);

        await Verify(response);
    }

    [Fact]
    public async Task AddStudentToTeam_ShouldReturnNotFound_WhenInvalidTeam()
    {
        var teamId = Guid.NewGuid();
        var request = new AddStudentToTeamRequest
        {
            Email = "new@ntnu.no"
        };

        var response = Client.PostAsJsonAsync($"teams/{teamId}/students", request);

        await Verify(response);
    }
}