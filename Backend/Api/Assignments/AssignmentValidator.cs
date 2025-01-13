using Database.Models;
using FluentValidation;

namespace Api.Assignments;

public class AssignmentValidator : AbstractValidator<Assignment>
{
    public AssignmentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.CourseId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
