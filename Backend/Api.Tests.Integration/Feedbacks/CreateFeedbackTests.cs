using Api.Feedbacks.Contracts;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Feedbacks;

public class CreateFeedbackTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateFeedback_ShouldCreateNoGradingFeedback_WhenValidRequest()
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
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            DeliveryId = delivery.Id,
            Comment = "Looks good to me"
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks.AnyAsync(f => f.DeliveryId == delivery.Id));
    }

    [Fact]
    public async Task CreateFeedback_ShouldCreateApprovalFeedback_WhenValidRequest()
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
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            DeliveryId = delivery.Id,
            Comment = "Looks good to me",
            IsApproved = true,
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks.AnyAsync(f => f.DeliveryId == delivery.Id));
    }

    [Fact]
    public async Task CreateFeedback_ShouldCreateLetterFeedback_WhenValidRequest()
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
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            DeliveryId = delivery.Id,
            Comment = "Looks good to me",
            LetterGrade = LetterGrade.C
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks.AnyAsync(f => f.DeliveryId == delivery.Id));
    }

    [Fact]
    public async Task CreateFeedback_ShouldCreatePointsFeedback_WhenValidRequest()
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
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            DeliveryId = delivery.Id,
            Comment = "Looks good to me",
            Points = 75
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks.AnyAsync(f => f.DeliveryId == delivery.Id));
    }

    [Fact]
    public async Task CreateFeedback_ShouldReturnBadRequest_WhenInvalidRequest()
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
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            DeliveryId = delivery.Id,
            Comment = "Looks good to me",
            IsApproved = true
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateFeedback_ShouldReturnBadRequest_WhenDueDateNotPassed()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.NoGrading,
            offset: TimeSpan.FromDays(7)
        );
        var delivery = ModelFactory.CreateStudentDelivery(assignment.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            DeliveryId = delivery.Id,
            Comment = "Looks good to me"
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateFeedback_ShouldReturnNotFound_WhenInvalidDelivery()
    {
        var deliveryId = Guid.NewGuid();

        var request = new CreateFeedbackRequest
        {
            DeliveryId = deliveryId,
            Comment = "Looks good to me"
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
    }
}