using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Courses;

public class GetAllCoursesByTeacherTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllCoursesByTeacher_ShouldReturnEmpty_WhenEmpty()
    {
        DbContext.AddRange([
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
        ]);

        var teacher = ModelFactory.GetValidTeacher();
        DbContext.Users.Add(teacher);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teachers/{teacher.Id}/courses");
        await Verify(response);
    }

    [Fact]
    public async Task GetAllCoursesByTeacher_ShouldReturnCourses_WhenCoursesExists()
    {
        List<Course> courses = [
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
        ];
        DbContext.AddRange(courses);

        var teacher = ModelFactory.GetValidTeacher();
        DbContext.Users.Add(teacher);

        DbContext.CourseTeachers.Add(ModelFactory.GetValidCourseTeacher(courses[0].Id, teacher.Id));
        DbContext.CourseTeachers.Add(ModelFactory.GetValidCourseTeacher(courses[1].Id, teacher.Id));
        DbContext.CourseTeachers.Add(ModelFactory.GetValidCourseTeacher(courses[2].Id, teacher.Id));

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teachers/{teacher.Id}/courses");
        await Verify(response);
    }

    [Fact]
    public async Task GetAllCoursesByTeacher_ShouldReturnNotFound_WhenInvalidTeacher()
    {
        var teacherId = Guid.NewGuid();

        var response = await Client.GetAsync($"teachers/{teacherId}/courses");
        await Verify(response);
    }
}