using Database.Models;

namespace Api.Tests.Integration.Students;

public class GetAllStudentsByCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllStudentsByCourse_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);
        await DbContext.SaveChangesAsync();

        var response = Client.GetAsync($"courses/{course.Id}/students");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllStudentsByCourse_ShouldReturnStudents_WhenStudentsExists()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        List<User> students = [
            ModelFactory.GetValidStudent("student1@ntnu.no"),
            ModelFactory.GetValidTeacher("student2@ntnu.no"),
            ModelFactory.GetValidTeacher("student3@ntnu.no")
        ];
        DbContext.Users.AddRange(students);

        DbContext.CourseStudents.AddRange([
            ModelFactory.GetValidCourseStudent(course.Id, students[0].Id),
            ModelFactory.GetValidCourseStudent(course.Id, students[1].Id),
            ModelFactory.GetValidCourseStudent(course.Id, students[2].Id)
        ]);

        await DbContext.SaveChangesAsync();

        var response = Client.GetAsync($"courses/{course.Id}/students");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllStudentsByCourse_ShouldNotFound_WhenInvalidCourse()
    {
        var id = Guid.NewGuid();

        var response = Client.GetAsync($"courses/{id}/students");

        await Verify(response);
    }
}