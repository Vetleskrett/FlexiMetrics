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

        var noGradingDeliveries = ModelFactory.CreateStudentDeliveries(noGradingAssignment.Id, students);
        var approvalDeliveries = ModelFactory.CreateStudentDeliveries(approvalAssignment.Id, students);
        var letterDeliveries = ModelFactory.CreateStudentDeliveries(letterAssignment.Id, students);
        var pointsDeliveries = ModelFactory.CreateStudentDeliveries(pointsAssignment.Id, students);

        ModelFactory.CreateFeedbacks(noGradingDeliveries);
        ModelFactory.CreateApprovalFeedbacks(approvalDeliveries);
        ModelFactory.CreateLetterFeedbacks(letterDeliveries);
        ModelFactory.CreatePointsFeedbacks(pointsDeliveries);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync("feedbacks");

        await Verify(response);
    }
}