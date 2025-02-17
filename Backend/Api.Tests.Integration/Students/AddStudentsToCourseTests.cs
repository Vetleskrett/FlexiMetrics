using Api.Students.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Students;

public class AddStudentsToCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
	[Fact]
	public async Task AddStudentsToCourse_ShouldAddStudentsToCourse_WhenValidRequest()
	{
		var course = ModelFactory.CreateCourse();
		var existingStudents = ModelFactory.CreateStudents(4);
		ModelFactory.CreateCourseStudent(course.Id, existingStudents[0].Id);
		ModelFactory.CreateCourseStudent(course.Id, existingStudents[1].Id);
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

		var response = await Client.PostAsJsonAsync($"courses/{course.Id}/students", request);

		await Verify(response);
		foreach (var email in request.Emails)
		{
			Assert.True(await DbContext.Users.AnyAsync(u => u.Email == email));

			Assert.True(await DbContext.CourseStudents.AnyAsync(cs =>
				cs.CourseId == course.Id &&
				cs.Student!.Email == email
			));
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

		var response = await Client.PostAsJsonAsync($"courses/{courseId}/students", request);

		await Verify(response);
	}
}