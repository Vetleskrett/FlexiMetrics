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

        RuleFor(x => x.RangeMin)
            .Must((field, min) =>
            {
                return field.Type != AssignmentDataType.Range || (field.Type == AssignmentDataType.Range && min is not null);
            })
            .WithMessage("'RangeMin' must not be empty when 'Type' is Range");

        RuleFor(x => x.RangeMin)
            .Must((field, min) =>
            {
                return field.Type == AssignmentDataType.Range || (field.Type != AssignmentDataType.Range && min is null);
            })
            .WithMessage("'RangeMin' must be null when 'Type' is not Range");

        RuleFor(x => x.RangeMax)
            .Must((field, max) =>
            {
                return field.Type != AssignmentDataType.Range || (field.Type == AssignmentDataType.Range && max is not null);
            })
            .WithMessage("'RangeMax' must not be empty when 'Type' is Range");

        RuleFor(x => x.RangeMax)
            .Must((field, max) =>
            {
                return field.Type == AssignmentDataType.Range || (field.Type != AssignmentDataType.Range && max is null);
            })
            .WithMessage("'RangeMax' must be null when 'Type' is not Range");
    }
}
