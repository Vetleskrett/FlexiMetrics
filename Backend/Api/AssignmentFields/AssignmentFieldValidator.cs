using Database.Models;
using FluentValidation;

namespace Api.AssignmentFields;

public class AssignmentFieldValidator : AbstractValidator<AssignmentField>
{
    public AssignmentFieldValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.AssignmentId)
            .NotEmpty();
    }
}
