﻿using Database.Models;
using FluentValidation;

namespace Api.Feedbacks;

public class FeedbackValidator : AbstractValidator<Feedback>
{
    public FeedbackValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Assignment)
            .NotEmpty();

        RuleFor(x => x)
            .Must(feedback =>
            {
                var gradingType = feedback.Assignment!.GradingType;
                if (feedback is ApprovalFeedback approvalFeedback)
                {
                    return gradingType == GradingType.ApprovalGrading;
                }
                if (feedback is LetterFeedback letterFeedback)
                {
                    return gradingType == GradingType.LetterGrading &&
                        Enum.IsDefined(letterFeedback.LetterGrade);
                }
                if (feedback is PointsFeedback pointsFeedback)
                {
                    return gradingType == GradingType.PointsGrading &&
                        pointsFeedback.Points >= 0 &&
                        pointsFeedback.Points <= feedback.Assignment.MaxPoints;
                }
                return gradingType == GradingType.NoGrading;
            })
            .WithMessage("Feedback grading must match assignment grading type");
    }
}