using Database.Models;
using FluentValidation;
using System.Text.RegularExpressions;

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

        RuleFor(x => x.Regex)
            .NotEqual(string.Empty);
    }
}
