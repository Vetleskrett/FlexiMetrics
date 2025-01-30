using Database.Models;
using FluentValidation;

namespace Api.Feedbacks;

public class FeedbackValidator : AbstractValidator<Feedback>
{
    public FeedbackValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
