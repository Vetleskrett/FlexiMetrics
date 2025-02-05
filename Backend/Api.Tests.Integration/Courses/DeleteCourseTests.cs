using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Courses;

public class DeleteCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteCourse_ShouldDeleteCourse_WhenValidCourse()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"courses/{course.Id}");

        await Verify(response);
        Assert.False(await DbContext.Courses.AnyAsync());
    }

    [Fact]
    public async Task DeleteCourse_ShouldDeleteAssignments_WhenValidCourse()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        DbContext.Assignments.AddRange([
            ModelFactory.GetValidAssignment(course.Id),
            ModelFactory.GetValidAssignment(course.Id),
            ModelFactory.GetValidAssignment(course.Id),
        ]);

        await DbContext.SaveChangesAsync();

        await Client.DeleteAsync($"courses/{course.Id}");

        Assert.False(await DbContext.Assignments.AnyAsync());
    }

    [Fact]
    public async Task DeleteCourse_ShouldDeleteTeams_WhenValidCourse()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        DbContext.Teams.AddRange([
            ModelFactory.GetValidTeam(course.Id, 1),
            ModelFactory.GetValidTeam(course.Id, 2),
            ModelFactory.GetValidTeam(course.Id, 3),
        ]);

        await DbContext.SaveChangesAsync();

        await Client.DeleteAsync($"courses/{course.Id}");

        Assert.False(await DbContext.Teams.AnyAsync());
    }

    [Fact]
    public async Task DeleteCourse_ShouldDeleteCourseTeachers_WhenValidCourse()
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

        await Client.DeleteAsync($"courses/{course.Id}");

        Assert.False(await DbContext.CourseTeachers.AnyAsync());
    }

    [Fact]
    public async Task DeleteCourse_ShouldDeleteCourseStudents_WhenValidCourse()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        List<User> students = [
            ModelFactory.GetValidStudent("student1@ntnu.no"),
            ModelFactory.GetValidStudent("student2@ntnu.no"),
            ModelFactory.GetValidStudent("student3@ntnu.no"),
        ];
        DbContext.Users.AddRange(students);

        DbContext.CourseStudents.AddRange([
            ModelFactory.GetValidCourseStudent(course.Id, students[0].Id),
            ModelFactory.GetValidCourseStudent(course.Id, students[1].Id),
            ModelFactory.GetValidCourseStudent(course.Id, students[2].Id),
        ]);

        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"courses/{course.Id}");

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