using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Feedbacks;

public class DeleteFeedbackTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteFeedback_ShouldDeleteFeedback_WhenValidFeedback()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(course.Id);
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        var feedback = ModelFactory.CreateFeedback(delivery.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"feedbacks/{feedback.Id}");

        await Verify(response);
        Assert.False(await DbContext.Feedbacks.AnyAsync(f => f.Id == feedback.Id));
    }

    [Fact]
    public async Task DeleteFeedback_ShouldReturnNotFound_WhenInvalidFeedback()
    {
        var feedbackId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"feedbacks/{feedbackId}");

        await Verify(response);
    }
}