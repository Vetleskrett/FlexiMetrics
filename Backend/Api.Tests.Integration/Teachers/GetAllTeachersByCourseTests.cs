namespace Api.Tests.Integration.Teachers;

public class GetAllTeachersByCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
	[Fact]
	public async Task GetAllTeachersByCourse_ShouldReturnEmpty_WhenEmpty()
	{
		var course = ModelFactory.CreateCourse();
		await DbContext.SaveChangesAsync();

		var response = await Client.GetAsync($"courses/{course.Id}/teachers");

		await Verify(response);
	}

	[Fact]
	public async Task GetAllTeachersByCourse_ShouldReturnTeachers_WhenTeachersExists()
	{
		var course = ModelFactory.CreateCourse();
		ModelFactory.CreateCourseTeachers(course.Id, 3);
		await DbContext.SaveChangesAsync();

		var response = await Client.GetAsync($"courses/{course.Id}/teachers");

		await Verify(response);
	}

	[Fact]
	public async Task GetAllTeachersByCourse_ShouldNotFound_WhenInvalidCourse()
	{
		var courseId = Guid.NewGuid();

		var response = await Client.GetAsync($"courses/{courseId}/teachers");

		await Verify(response);
	}
}