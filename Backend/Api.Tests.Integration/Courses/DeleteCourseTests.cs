using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Courses;

public class DeleteCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteCourse_ShouldDeleteCourse_WhenValidCourse()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"courses/{course.Id}");

        await Verify(response);
        Assert.False(await DbContext.Courses.AnyAsync());
    }

    [Fact]
    public async Task DeleteCourse_ShouldDeleteAssignments_WhenValidCourse()
    {
        var course = ModelFactory.CreateCourse();
        ModelFactory.CreateAssignments(course.Id, 3);
        await DbContext.SaveChangesAsync();

        await Client.DeleteAsync($"courses/{course.Id}");

        Assert.False(await DbContext.Assignments.AnyAsync());
    }

    [Fact]
    public async Task DeleteCourse_ShouldDeleteTeams_WhenValidCourse()
    {
        var course = ModelFactory.CreateCourse();
        ModelFactory.CreateTeams(course.Id, 3);
        await DbContext.SaveChangesAsync();

        await Client.DeleteAsync($"courses/{course.Id}");

        Assert.False(await DbContext.Teams.AnyAsync());
    }

    [Fact]
    public async Task DeleteCourse_ShouldDeleteCourseTeachers_WhenValidCourse()
    {
        var course = ModelFactory.CreateCourse();
        ModelFactory.CreateCourseTeachers(course.Id, 3);
        await DbContext.SaveChangesAsync();

        await Client.DeleteAsync($"courses/{course.Id}");

        Assert.False(await DbContext.CourseTeachers.AnyAsync());
    }

    [Fact]
    public async Task DeleteCourse_ShouldDeleteCourseStudents_WhenValidCourse()
    {
        var course = ModelFactory.CreateCourse();
        ModelFactory.CreateCourseStudents(course.Id, 3);
        await DbContext.SaveChangesAsync();

        await Client.DeleteAsync($"courses/{course.Id}");

        Assert.False(await DbContext.CourseStudents.AnyAsync());
    }

    [Fact]
    public async Task DeleteCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var id = Guid.NewGuid();

        var response = await Client.DeleteAsync($"courses/{id}");

        await Verify(response);
    }
}