using Database.Models;
using FluentValidation;
using System.Text.Json;

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

                try
                {
                    object? value = assignmentField.Type switch
                    {
                        AssignmentDataType.String => field.JsonValue?.Deserialize<string>(),
                        AssignmentDataType.Integer => field.JsonValue?.Deserialize<int>(),
                        AssignmentDataType.Double => field.JsonValue?.Deserialize<double>(),
                        AssignmentDataType.Boolean => field.JsonValue?.Deserialize<bool>(),
                        _ => null,
                    };
                    return value is not null;
                }
                catch
                {
                    return false;
                }
            })
            .WithMessage("Delivery field must match assignment field from the assignment");
    }
}
