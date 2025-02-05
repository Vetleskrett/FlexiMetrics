using Database.Models;

namespace Api.Tests.Integration.Courses;

public class GetAllCoursesByStudentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllCoursesByStudent_ShouldReturnEmpty_WhenEmpty()
    {
        DbContext.AddRange([
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
        ]);

        var student = ModelFactory.GetValidStudent();
        DbContext.Users.Add(student);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/courses");
        await Verify(response);
    }

    [Fact]
    public async Task GetAllCoursesByStudent_ShouldReturnCourses_WhenCoursesExists()
    {
        List<Course> courses = [
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
            ModelFactory.GetValidCourse(),
        ];
        DbContext.AddRange(courses);

        var student = ModelFactory.GetValidStudent();
        DbContext.Users.Add(student);

        DbContext.CourseStudents.Add(ModelFactory.GetValidCourseStudent(courses[0].Id, student.Id));
        DbContext.CourseStudents.Add(ModelFactory.GetValidCourseStudent(courses[1].Id, student.Id));
        DbContext.CourseStudents.Add(ModelFactory.GetValidCourseStudent(courses[2].Id, student.Id));

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/courses");
        await Verify(response);
    }

    [Fact]
    public async Task GetAllCoursesByStudent_ShouldReturnNotFound_WhenInvalidStudent()
    {
        var studentId = Guid.NewGuid();

        var response = await Client.GetAsync($"students/{studentId}/courses");
        await Verify(response);
    }
}