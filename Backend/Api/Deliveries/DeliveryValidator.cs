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
            .NotNull();

        RuleFor(x => x.Fields)
            .Must((delivery, fields) =>
            {
                foreach (var assignmentField in delivery.Assignment!.Fields!)
                {
                    if (!fields!.Any(f => f.AssignmentFieldId == assignmentField.Id))
                    {
                        return false;
                    }
                }
                return true;
            })
            .WithMessage("Delivery must contain one delivery field per assignment field");

        RuleFor(x => x.Fields)
            .Must((delivery, fields) =>
            {
                foreach (var field in fields!)
                {
                    if (!delivery.Assignment!.Fields!.Any(f => f.Id == field.AssignmentFieldId))
                    {
                        return false;
                    }
                }
                return true;
            })
            .WithMessage("Delivery can only contain delivery fields for assignment fields from assignment");

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
            .WithMessage("Delivery field data type must match assignment field data type");
    }
}
