using Api.Teams.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Teams;

public class BulkAddStudentsToTeamsTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task BulkAddStudentsToTeams_ShouldAddStudentsToTeams_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var inCourseStudents = ModelFactory.CreateCourseStudents(course.Id, 4);
        var teams = ModelFactory.CreateTeamsWithStudents(course.Id, inCourseStudents, 2);
        var existingStudent = ModelFactory.CreateStudent("existing@ntnu.no");
        await DbContext.SaveChangesAsync();

        var request = new BulkAddStudentsToTeamsRequest
        {
            CourseId = course.Id,
            Teams = [
                new TeamRequest
                {
                    TeamNr = teams[0].TeamNr,
                    Emails = [
                        teams[0].Students[0].Email,
                        "new1@ntnu.no",
                        existingStudent.Email
                    ]
                },
                new TeamRequest
                {
                    TeamNr = 100,
                    Emails = [
                        "new2@ntnu.no",
                        "new3@ntnu.no"
                    ]
                }
            ]
        };

        var response = Client.PostAsJsonAsync("teams/bulk", request);

        await Verify(response);
    }

    [Fact]
    public async Task BulkAddStudentsToTeams_ShouldCreateNewTeamsStudentAndAddToCourse_WhenNotExist()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();

        var request = new BulkAddStudentsToTeamsRequest
        {
            CourseId = course.Id,
            Teams = [
                new TeamRequest
                {
                    TeamNr = 1,
                    Emails = [
                        "new1@ntnu.no",
                        "new2@ntnu.no"
                    ]
                },
                new TeamRequest
                {
                    TeamNr = 2,
                    Emails = [
                        "new3@ntnu.no",
                        "new4@ntnu.no"
                    ]
                }
            ]
        };

        await Client.PostAsJsonAsync("teams/bulk", request);

        Assert.True(await DbContext.Teams.AnyAsync(t => t.CourseId == course.Id && t.TeamNr == 1));
        Assert.True(await DbContext.Teams.AnyAsync(t => t.CourseId == course.Id && t.TeamNr == 2));

        Assert.True(await DbContext.Users.AnyAsync(u => u.Email == "new1@ntnu.no"));
        Assert.True(await DbContext.Users.AnyAsync(u => u.Email == "new2@ntnu.no"));
        Assert.True(await DbContext.Users.AnyAsync(u => u.Email == "new3@ntnu.no"));
        Assert.True(await DbContext.Users.AnyAsync(u => u.Email == "new4@ntnu.no"));

        Assert.True(await DbContext.CourseStudents.AnyAsync(cs => cs.CourseId == course.Id && cs.Student!.Email == "new1@ntnu.no"));
        Assert.True(await DbContext.CourseStudents.AnyAsync(cs => cs.CourseId == course.Id && cs.Student!.Email == "new2@ntnu.no"));
        Assert.True(await DbContext.CourseStudents.AnyAsync(cs => cs.CourseId == course.Id && cs.Student!.Email == "new3@ntnu.no"));
        Assert.True(await DbContext.CourseStudents.AnyAsync(cs => cs.CourseId == course.Id && cs.Student!.Email == "new4@ntnu.no"));
    }

    [Fact]
    public async Task BulkAddStudentsToTeams_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var request = new BulkAddStudentsToTeamsRequest
        {
            CourseId = Guid.NewGuid(),
            Teams = [
                new TeamRequest
                {
                    TeamNr = 1,
                    Emails = [
                        "new1@ntnu.no",
                        "new2@ntnu.no"
                    ]
                }
            ]
        };

        var response = await Client.PostAsJsonAsync("teams/bulk", request);

        await Verify(response);
    }
}