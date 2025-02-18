namespace Api.Tests.Integration.Feedbacks;

public class GetFeedbackByStudentAssignmentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
	[Fact]
	public async Task GetFeedbackByStudentAssignment_ShouldReturnFeedback_WhenStudentAssignment()
	{
		var course = ModelFactory.CreateCourse();
		var student = ModelFactory.CreateStudent();
		ModelFactory.CreateCourseStudent(course.Id, student.Id);
		var assignment = ModelFactory.CreateAssignment(course.Id);
		ModelFactory.CreateFeedback(assignment.Id, student.Id, null);
		await DbContext.SaveChangesAsync();

		var response = await Client.GetAsync($"students/{student.Id}/assignments/{assignment.Id}/feedbacks");

		await Verify(response);
	}

	[Fact]
	public async Task GetFeedbackByStudentAssignment_ShouldReturnFeedback_WhenTeamAssignment()
	{
		var course = ModelFactory.CreateCourse();
		var students = ModelFactory.CreateCourseStudents(course.Id, 2);
		var team = ModelFactory.CreateTeam(course.Id, students: students);
		var assignment = ModelFactory.CreateAssignment(course.Id);
		ModelFactory.CreateFeedback(assignment.Id, null, team.Id);
		await DbContext.SaveChangesAsync();

		var response = await Client.GetAsync($"students/{students[0].Id}/assignments/{assignment.Id}/feedbacks");

		await Verify(response);
	}

	[Fact]
	public async Task GetFeedbackByStudentAssignment_ShouldReturnBadRequest_WhenStudentNotInCourse()
	{
		var course = ModelFactory.CreateCourse();
		var student = ModelFactory.CreateStudent();
		var assignment = ModelFactory.CreateAssignment(course.Id);
		await DbContext.SaveChangesAsync();

		var response = await Client.GetAsync($"students/{student.Id}/assignments/{assignment.Id}/feedbacks");

		await Verify(response);
	}

	[Fact]
	public async Task GetFeedbackByStudentAssignment_ShouldReturnNoContent_WhenNoFeedbackExists()
	{
		var course = ModelFactory.CreateCourse();
		var student = ModelFactory.CreateStudent();
		ModelFactory.CreateCourseStudent(course.Id, student.Id);
		var assignment = ModelFactory.CreateAssignment(course.Id);
		ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
		await DbContext.SaveChangesAsync();

		var response = await Client.GetAsync($"students/{student.Id}/assignments/{assignment.Id}/feedbacks");

		await Verify(response);
	}

	[Fact]
	public async Task GetFeedbackByStudentAssignment_ShouldReturnNotFound_WhenInvalidStudent()
	{
		var course = ModelFactory.CreateCourse();
		var assignment = ModelFactory.CreateAssignment(course.Id);
		await DbContext.SaveChangesAsync();
		var studentId = Guid.NewGuid();

		var response = await Client.GetAsync($"students/{studentId}/assignments/{assignment.Id}/feedbacks");

		await Verify(response);
	}

	[Fact]
	public async Task GetFeedbackByStudentAssignment_ShouldReturnNotFound_WhenInvalidAssignment()
	{
		var course = ModelFactory.CreateCourse();
		var student = ModelFactory.CreateStudent();
		ModelFactory.CreateCourseStudent(course.Id, student.Id);
		await DbContext.SaveChangesAsync();
		var assignmentId = Guid.NewGuid();

		var response = await Client.GetAsync($"students/{student.Id}/assignments/{assignmentId}/feedbacks");

		await Verify(response);
	}
}