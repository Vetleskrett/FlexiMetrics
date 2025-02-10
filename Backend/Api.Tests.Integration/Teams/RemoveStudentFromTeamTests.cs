using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Teams;

public class RemoveStudentFromTeamTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task RemoveStudentFromTeam_ShouldRemoveStudentFromTeam_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var team = ModelFactory.CreateTeam(course.Id, 1, [student]);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"teams/{team.Id}/students/{student.Id}");

        await Verify(response);

        Assert.False(await DbContext.Teams.AnyAsync(t => t.Id == team.Id && t.Students.Any(s => s.Id == student.Id)));
    }

    [Fact]
    public async Task RemoveStudentFromTeam_ShouldReturnBadRequest_WhenStudentNotInTeam()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var team = ModelFactory.CreateTeam(course.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"teams/{team.Id}/students/{student.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task RemoveStudentFromTeam_ShouldReturnNotFound_WhenInvalidStudent()
    {
        var course = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(course.Id);
        await DbContext.SaveChangesAsync();
        var studentId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"teams/{team.Id}/students/{studentId}");

        await Verify(response);
    }

    [Fact]
    public async Task RemoveStudentFromTeam_ShouldReturnNotFound_WhenInvalidTeam()
    {
        var student = ModelFactory.CreateStudent();
        await DbContext.SaveChangesAsync();
        var teamId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"teams/{teamId}/students/{student.Id}");

        await Verify(response);
    }
}