using Database.Models;
using FluentValidation;

namespace Api.Deliveries;

public class DeliveryValidator : AbstractValidator<Delivery>
{
    public DeliveryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.AssignmentId)
            .NotEmpty();

        RuleFor(x => x.Assignment)
            .NotEmpty();

        RuleFor(x => x.Fields)
            .NotEmpty();

        RuleForEach(x => x.Fields)
            .Must((delivery, field) =>
            {
                return delivery.Assignment!.Fields!.Any(f => f.Id == field.AssignmentFieldId);
            })
            .WithMessage("Delivery field must match assignment field from the assignment");

        RuleFor(x => x.Fields)
            .Must((delivery, fields) =>
            {
                return fields!.Count == delivery.Assignment!.Fields!.Count;
            })
            .WithMessage("Delivery must contain one delivery field per assignment field");

        RuleFor(x => x.Fields)
            .Must((delivery, fields) =>
            {
                return fields!.DistinctBy(f => f.AssignmentFieldId).Count() == fields!.Count;
            })
            .WithMessage("Delivery can only contain one delivery field per assignment field");
    }
}
