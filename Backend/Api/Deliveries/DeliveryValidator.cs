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

        RuleForEach(x => x.Fields)
            .Must((delivery, field) =>
            {
                var assignmentField = delivery.Assignment!.Fields!.FirstOrDefault(f => f.Id == field.AssignmentFieldId);
                if (assignmentField is null)
                {
                    return false;
                }

                return assignmentField.Type switch
                {
                    AssignmentDataType.String => field.Value.GetType() == typeof(string),
                    AssignmentDataType.Integer => field.Value.GetType() == typeof(int),
                    AssignmentDataType.Double => field.Value.GetType() == typeof(double),
                    AssignmentDataType.Boolean => field.Value.GetType() == typeof(bool),
                    _ => false,
                };
            })
            .WithMessage("Delivery field must match assignment field from the assignment");
    }
}
