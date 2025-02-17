using Api.Feedbacks.Contracts;
using Database.Models;

namespace Api.Feedbacks;

public static class FeedbackMapping
{
    public static Feedback MapToFeedback(this CreateFeedbackRequest request)
    {
        if (request.IsApproved is not null)
        {
            return new ApprovalFeedback
            {
                Id = Guid.NewGuid(),
                Comment = request.Comment,
                AssignmentId = request.AssignmentId,
                StudentId = request.StudentId,
                TeamId = request.TeamId,
                IsApproved = request.IsApproved.Value,
            };
        }

        if (request.LetterGrade is not null)
        {
            return new LetterFeedback
            {
                Id = Guid.NewGuid(),
                Comment = request.Comment,
                AssignmentId = request.AssignmentId,
                StudentId = request.StudentId,
                TeamId = request.TeamId,
                LetterGrade = request.LetterGrade.Value,
            };
        }

        if (request.Points is not null)
        {
            return new PointsFeedback
            {
                Id = Guid.NewGuid(),
                Comment = request.Comment,
                AssignmentId = request.AssignmentId,
                StudentId = request.StudentId,
                TeamId = request.TeamId,
                Points = request.Points.Value,
            };
        }

        return new Feedback
        {
            Id = Guid.NewGuid(),
            Comment = request.Comment,
            AssignmentId = request.AssignmentId,
            StudentId = request.StudentId,
            TeamId = request.TeamId,
        };
    }

    public static Feedback MapToFeedback
    (
        this UpdateFeedbackRequest request,
        Guid id,
        Guid assignmentId,
        Guid? studentId,
        Guid? teamId
    )
    {
        if (request.IsApproved is not null)
        {
            return new ApprovalFeedback
            {
                Id = id,
                Comment = request.Comment,
                AssignmentId = assignmentId,
                StudentId = studentId,
                TeamId = teamId,
                IsApproved = request.IsApproved.Value,
            };
        }

        if (request.LetterGrade is not null)
        {
            return new LetterFeedback
            {
                Id = id,
                Comment = request.Comment,
                AssignmentId = assignmentId,
                StudentId = studentId,
                TeamId = teamId,
                LetterGrade = request.LetterGrade.Value,
            };
        }

        if (request.Points is not null)
        {
            return new PointsFeedback
            {
                Id = id,
                Comment = request.Comment,
                AssignmentId = assignmentId,
                StudentId = studentId,
                TeamId = teamId,
                Points = request.Points.Value,
            };
        }

        return new Feedback
        {
            Id = id,
            Comment = request.Comment,
            AssignmentId = assignmentId,
            StudentId = studentId,
            TeamId = teamId,
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
                AssignmentId = feedback.AssignmentId,
                StudentId = feedback.StudentId,
                TeamId = feedback.TeamId,
                IsApproved = approval.IsApproved,
            };
        }

        if (feedback is LetterFeedback letter)
        {
            return new FeedbackResponse
            {
                Id = feedback.Id,
                Comment = feedback.Comment,
                AssignmentId = feedback.AssignmentId,
                StudentId = feedback.StudentId,
                TeamId = feedback.TeamId,
                LetterGrade = letter.LetterGrade,
            };
        }

        if (feedback is PointsFeedback points)
        {
            return new FeedbackResponse
            {
                Id = feedback.Id,
                Comment = feedback.Comment,
                AssignmentId = feedback.AssignmentId,
                StudentId = feedback.StudentId,
                TeamId = feedback.TeamId,
                Points = points.Points,
            };
        }

        return new FeedbackResponse
        {
            Id = feedback.Id,
            Comment = feedback.Comment,
            AssignmentId = feedback.AssignmentId,
            StudentId = feedback.StudentId,
            TeamId = feedback.TeamId,
        };
    }

    public static List<FeedbackResponse> MapToResponse(this IEnumerable<Feedback> feedbacks)
    {
        return feedbacks.Select(feedback => feedback.MapToResponse()).ToList();
    }
}
