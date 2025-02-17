using Database.Models;

namespace Api.Tests.Integration.Feedbacks;

public class GetAllFeedbacksTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllFeedbacks_ShouldReturnEmpty_WhenEmpty()
    {
        var response = await Client.GetAsync("feedbacks");
        await Verify(response);
    }

    [Fact]
    public async Task GetAllFeedbacks_ShouldReturnFeedbacks_WhenFeedbacksExist()
    {
        var course = ModelFactory.CreateCourse();
        var students = ModelFactory.CreateCourseStudents(course.Id, 3);

        var noGradingAssignment = ModelFactory.CreateAssignment(course.Id, gradingType: GradingType.NoGrading);
        var approvalAssignment = ModelFactory.CreateAssignment(course.Id, gradingType: GradingType.ApprovalGrading);
        var letterAssignment = ModelFactory.CreateAssignment(course.Id, gradingType: GradingType.LetterGrading);
        var pointsAssignment = ModelFactory.CreateAssignment(course.Id, gradingType: GradingType.PointsGrading, maxPoints: 100);

        ModelFactory.CreateFeedbacks(noGradingAssignment.Id, students);
        ModelFactory.CreateApprovalFeedbacks(approvalAssignment.Id, students);
        ModelFactory.CreateLetterFeedbacks(letterAssignment.Id, students);
        ModelFactory.CreatePointsFeedbacks(pointsAssignment.Id, students);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync("feedbacks");

        await Verify(response);
    }
}