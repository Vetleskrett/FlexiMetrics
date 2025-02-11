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
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        var feedback = ModelFactory.CreateFeedback(delivery.Id);
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
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        var feedback = ModelFactory.CreateApprovalFeedback(delivery.Id);
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
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        var feedback = ModelFactory.CreateLetterFeedback(delivery.Id);
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
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        var feedback = ModelFactory.CreatePointsFeedback(delivery.Id);
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