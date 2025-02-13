using Database.Models;
using FluentValidation;

namespace Api.Assignments;

public class AssignmentValidator : AbstractValidator<Assignment>
{
    public AssignmentValidator(IValidator<AssignmentField> assignmentFieldValidator)
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.DueDate)
            .NotEmpty();

        RuleFor(x => x.CollaborationType)
            .IsInEnum();

        RuleFor(x => x.GradingType)
            .IsInEnum();

        RuleFor(x => x.Description)
            .NotNull();

        RuleFor(x => x.CourseId)
            .NotEmpty();

        RuleForEach(x => x.Fields)
            .NotNull()
            .SetValidator(assignmentFieldValidator);
    }
}
