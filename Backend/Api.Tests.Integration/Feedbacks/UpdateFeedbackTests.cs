using Api.Feedbacks.Contracts;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Feedbacks;

public class UpdateFeedbackTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateFeedback_ShouldUpdateNoGradingFeedback_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.NoGrading,
            offset: TimeSpan.FromDays(-7)
        );
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        var feedback = ModelFactory.CreateFeedback(delivery.Id);
        await DbContext.SaveChangesAsync();

        var request = new UpdateFeedbackRequest
        {
            Comment = "Looks even better to me"
        };

        var response = await Client.PutAsJsonAsync($"feedbacks/{feedback.Id}", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks
            .OfType<Feedback>()
            .AnyAsync(f =>
                f.Id == feedback.Id &&
                f.DeliveryId == delivery.Id &&
                f.Comment == request.Comment
            )
        );
    }

    [Fact]
    public async Task UpdateFeedback_ShouldUpdateApprovalFeedback_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.ApprovalGrading,
            offset: TimeSpan.FromDays(-7)
        );
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        var feedback = ModelFactory.CreateApprovalFeedback(delivery.Id);
        await DbContext.SaveChangesAsync();

        var request = new UpdateFeedbackRequest
        {
            Comment = "Looks even better to me",
            IsApproved = false,
        };

        var response = await Client.PutAsJsonAsync($"feedbacks/{feedback.Id}", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks
            .OfType<ApprovalFeedback>()
            .AnyAsync(f =>
                f.Id == feedback.Id &&
                f.DeliveryId == delivery.Id &&
                f.Comment == request.Comment &&
                f.IsApproved == request.IsApproved
            )
        );
    }

    [Fact]
    public async Task UpdateFeedback_ShouldUpdateLetterFeedback_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.LetterGrading,
            offset: TimeSpan.FromDays(-7)
        );
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        var feedback = ModelFactory.CreateLetterFeedback(delivery.Id);
        await DbContext.SaveChangesAsync();

        var request = new UpdateFeedbackRequest
        {
            Comment = "Looks even better to me",
            LetterGrade = LetterGrade.B
        };

        var response = await Client.PutAsJsonAsync($"feedbacks/{feedback.Id}", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks
            .OfType<LetterFeedback>()
            .AnyAsync(f =>
                f.Id == feedback.Id &&
                f.DeliveryId == delivery.Id &&
                f.Comment == request.Comment &&
                f.LetterGrade == request.LetterGrade
            )
        );
    }

    [Fact]
    public async Task UpdateFeedback_ShouldUpdatePointsFeedback_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.PointsGrading,
            maxPoints: 100,
            offset: TimeSpan.FromDays(-7)
        );
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        var feedback = ModelFactory.CreatePointsFeedback(delivery.Id);
        await DbContext.SaveChangesAsync();

        var request = new UpdateFeedbackRequest
        {
            Comment = "Looks even better to me",
            Points = 95
        };

        var response = await Client.PutAsJsonAsync($"feedbacks/{feedback.Id}", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks
            .OfType<PointsFeedback>()
            .AnyAsync(f =>
                f.Id == feedback.Id &&
                f.DeliveryId == delivery.Id &&
                f.Comment == request.Comment &&
                f.Points == request.Points
            )
        );
    }

    [Fact]
    public async Task UpdateFeedback_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.LetterGrading,
            offset: TimeSpan.FromDays(-7)
        );
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        var feedback = ModelFactory.CreateLetterFeedback(delivery.Id);
        await DbContext.SaveChangesAsync();

        var request = new UpdateFeedbackRequest
        {
            Comment = "Looks even better to me",
            IsApproved = true
        };

        var response = await Client.PutAsJsonAsync($"feedbacks/{feedback.Id}", request);

        await Verify(response);
    }

    [Fact]
    public async Task UpdateFeedback_ShouldReturnNotFound_WhenInvalidFeedback()
    {
        var request = new UpdateFeedbackRequest
        {
            Comment = "Looks even better to me"
        };
        var id = Guid.NewGuid();

        var response = await Client.PutAsJsonAsync($"feedbacks/{id}", request);

        await Verify(response);
    }
}