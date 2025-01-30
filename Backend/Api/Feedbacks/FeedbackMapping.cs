using Api.Feedbacks.Contracts;
using Database.Models;

namespace Api.Feedbacks;

public static class FeedbackMapping
{
    public static Feedback MapToFeedback(this CreateFeedbackRequest request)
    {
        if (request.Grading is ApprovalGradingRequest approval)
        {
            return new ApprovalFeedback
            {
                Id = Guid.NewGuid(),
                Comment = request.Comment,
                DeliveryId = request.DeliveryId,
                IsApproved = approval.IsApproved,
            };
        }

        if (request.Grading is LetterGradingRequest letter)
        {
            return new LetterFeedback
            {
                Id = Guid.NewGuid(),
                Comment = request.Comment,
                DeliveryId = request.DeliveryId,
                LetterGrade = letter.LetterGrade,
            };
        }

        if (request.Grading is PointsGradingRequest points)
        {
            return new PointsFeedback
            {
                Id = Guid.NewGuid(),
                Comment = request.Comment,
                DeliveryId = request.DeliveryId,
                Points = points.Points,
            };
        }

        return new Feedback
        {
            Id = Guid.NewGuid(),
            Comment = request.Comment,
            DeliveryId = request.DeliveryId,
        };
    }

    public static Feedback MapToFeedback(this UpdateFeedbackRequest request, Guid id)
    {
        if (request.Grading is ApprovalGradingRequest approval)
        {
            return new ApprovalFeedback
            {
                Id = id,
                Comment = request.Comment,
                DeliveryId = request.DeliveryId,
                IsApproved = approval.IsApproved,
            };
        }

        if (request.Grading is LetterGradingRequest letter)
        {
            return new LetterFeedback
            {
                Id = id,
                Comment = request.Comment,
                DeliveryId = request.DeliveryId,
                LetterGrade = letter.LetterGrade,
            };
        }

        if (request.Grading is PointsGradingRequest points)
        {
            return new PointsFeedback
            {
                Id = id,
                Comment = request.Comment,
                DeliveryId = request.DeliveryId,
                Points = points.Points,
            };
        }

        return new Feedback
        {
            Id = id,
            Comment = request.Comment,
            DeliveryId = request.DeliveryId,
        };
    }

    public static FeedbackResponse MapToResponse(this Feedback feedback)
    {
        if (feedback is ApprovalFeedback approval)
        {
            return new FeedbackResponse
            {
                Id = feedback.Id,
                Comment = feedback.Comment,
                DeliveryId = feedback.DeliveryId,
                IsApproved = approval.IsApproved,
            };
        }

        if (feedback is LetterFeedback letter)
        {
            return new FeedbackResponse
            {
                Id = feedback.Id,
                Comment = feedback.Comment,
                DeliveryId = feedback.DeliveryId,
                LetterGrade = letter.LetterGrade,
            };
        }

        if (feedback is PointsFeedback points)
        {
            return new FeedbackResponse
            {
                Id = feedback.Id,
                Comment = feedback.Comment,
                DeliveryId = feedback.DeliveryId,
                Points = points.Points,
            };
        }

        return new FeedbackResponse
        {
            Id = feedback.Id,
            Comment = feedback.Comment,
            DeliveryId = feedback.DeliveryId,
        };
    }

    public static IEnumerable<FeedbackResponse> MapToResponse(this IEnumerable<Feedback> feedbacks)
    {
        return feedbacks.Select(feedback => feedback.MapToResponse());
    }
}
