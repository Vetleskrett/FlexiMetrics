using Database.Models;

namespace Api.Tests.Integration.Teachers;

public class GetAllTeachersByCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllTeachersByCourse_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);
        await DbContext.SaveChangesAsync();

        var response = Client.GetAsync($"courses/{course.Id}/teachers");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllTeachersByCourse_ShouldReturnTeachers_WhenTeachersExists()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        List<User> teachers = [
            ModelFactory.GetValidTeacher("teacher1@ntnu.no"),
            ModelFactory.GetValidTeacher("teacher2@ntnu.no"),
            ModelFactory.GetValidTeacher("teacher3@ntnu.no")
        ];
        DbContext.Users.AddRange(teachers);

        DbContext.CourseTeachers.AddRange([
            ModelFactory.GetValidCourseTeacher(course.Id, teachers[0].Id),
            ModelFactory.GetValidCourseTeacher(course.Id, teachers[1].Id),
            ModelFactory.GetValidCourseTeacher(course.Id, teachers[2].Id)
        ]);

        await DbContext.SaveChangesAsync();

        var response = Client.GetAsync($"courses/{course.Id}/teachers");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllTeachersByCourse_ShouldNotFound_WhenInvalidCourse()
    {
        var id = Guid.NewGuid();

        var response = Client.GetAsync($"courses/{id}/teachers");

        await Verify(response);
    }
}