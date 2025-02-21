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

        RuleFor(x => x.Regex)
            .NotEqual(string.Empty);

        RuleFor(x => x.SubType)
            .IsInEnum()
            .NotEqual(AssignmentDataType.List)
            .NotEqual(AssignmentDataType.File)
            .When(x => x.Type == AssignmentDataType.List);

        RuleFor(x => x.SubType)
            .Null()
            .When(x => x.Type != AssignmentDataType.List);
    }
}
