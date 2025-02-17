using Database.Models;

namespace Api.Tests.Integration.Feedbacks;

public class GetFeedbackTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetFeedback_ShouldReturnNoGradingFeedback_WhenNoGradingFeedbackExists()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, gradingType: GradingType.NoGrading);
        var feedback = ModelFactory.CreateFeedback(assignment.Id, student.Id, null);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"feedbacks/{feedback.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetFeedback_ShouldReturnApprovalFeedback_WhenApprovalFeedbackExists()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, gradingType: GradingType.ApprovalGrading);
        var feedback = ModelFactory.CreateApprovalFeedback(assignment.Id, student.Id, null);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"feedbacks/{feedback.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetFeedback_ShouldReturnLetterFeedback_WhenLetterFeedbackExists()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, gradingType: GradingType.LetterGrading);
        var feedback = ModelFactory.CreateLetterFeedback(assignment.Id, student.Id, null);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"feedbacks/{feedback.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetFeedback_ShouldReturnPointsFeedback_WhenPointsFeedbackExists()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id, gradingType: GradingType.PointsGrading, maxPoints: 100);
        var feedback = ModelFactory.CreatePointsFeedback(assignment.Id, student.Id, null);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"feedbacks/{feedback.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetFeedback_ShouldReturnNotFound_WhenInvalidFeedback()
    {
        var feedbackId = Guid.NewGuid();

        var response = await Client.GetAsync($"feedbacks/{feedbackId}");

        await Verify(response);
    }
}