using Api.Students.Contracts;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Students;

public class AddStudentsToCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task AddStudentsToCourse_ShouldAddStudentsToCourse_WhenValidRequest()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        List<User> existingStudents = [
            ModelFactory.GetValidStudent("existing1@ntnu.no"),
            ModelFactory.GetValidStudent("existing2@ntnu.no"),
            ModelFactory.GetValidStudent("existing3@ntnu.no"),
            ModelFactory.GetValidStudent("existing4@ntnu.no"),
        ];
        DbContext.Users.AddRange(existingStudents);

        DbContext.CourseStudents.AddRange([
            ModelFactory.GetValidCourseStudent(course.Id, existingStudents[0].Id),
            ModelFactory.GetValidCourseStudent(course.Id, existingStudents[1].Id),
        ]);

        await DbContext.SaveChangesAsync();

        var request = new AddStudentsToCourseRequest
        {
            Emails = [
                existingStudents[0].Email,
                existingStudents[1].Email,
                existingStudents[2].Email,
                existingStudents[3].Email,
                "new1@ntnu.no",
                "new2@ntnu.no",
            ]
        };

        var response = Client.PostAsJsonAsync($"courses/{course.Id}/students", request);

        await Verify(response);
        foreach (var email in request.Emails)
        {
            Assert.True(
                await DbContext.Users.AnyAsync(u => u.Email == email)
            );

            Assert.True(
                await DbContext.CourseStudents
                    .Where(cs => cs.CourseId == course.Id)
                    .AnyAsync(cs => cs.Student!.Email == email)
            );
        }
    }

    [Fact]
    public async Task AddStudentsToCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var request = new AddStudentsToCourseRequest
        {
            Emails = [
                "student1@ntnu.no",
                "student2@ntnu.no",
                "student3@ntnu.no",
            ]
        };

        var courseId = Guid.NewGuid();

        var response = Client.PostAsJsonAsync($"courses/{courseId}/students", request);

        await Verify(response);
    }
}