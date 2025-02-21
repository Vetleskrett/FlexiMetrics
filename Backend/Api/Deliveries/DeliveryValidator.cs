using Database.Models;
using FluentValidation;
using System.Text.Json;
using System.Text.RegularExpressions;

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
                        AssignmentDataType.ShortText => field.JsonValue?.Deserialize<string>(),
                        AssignmentDataType.LongText => field.JsonValue?.Deserialize<string>(),
                        AssignmentDataType.Integer => field.JsonValue?.Deserialize<int>(),
                        AssignmentDataType.Float => field.JsonValue?.Deserialize<double>(),
                        AssignmentDataType.Boolean => field.JsonValue?.Deserialize<bool>(),
                        AssignmentDataType.URL => field.JsonValue?.Deserialize<string>(),
                        AssignmentDataType.File => field.JsonValue?.Deserialize<FileMetadata>(),
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

        RuleForEach(x => x.Fields)
            .Must((delivery, field) =>
            {
                var assignmentField = delivery.Assignment!.Fields!.FirstOrDefault(f => f.Id == field.AssignmentFieldId);
                if (assignmentField?.Min is not null)
                {
                    switch(assignmentField.Type)
                    {
                        case AssignmentDataType.Integer:
                            var intValue = field.JsonValue?.Deserialize<int>();
                            return intValue is not null && intValue.Value >= assignmentField.Min;
                        case AssignmentDataType.Float:
                            var floatValue = field.JsonValue?.Deserialize<double>();
                            return floatValue is not null && floatValue.Value >= assignmentField.Min;
                        default:
                            return true;
                    }
                }
                return true;
            })
            .WithMessage("Delivery field value must be between min and max");

        RuleForEach(x => x.Fields)
            .Must((delivery, field) =>
            {
                var assignmentField = delivery.Assignment!.Fields!.FirstOrDefault(f => f.Id == field.AssignmentFieldId);
                if (assignmentField?.Min is not null)
                {
                    switch (assignmentField.Type)
                    {
                        case AssignmentDataType.ShortText:
                        case AssignmentDataType.LongText:
                            var stringValue = field.JsonValue?.Deserialize<string>();
                            return stringValue is not null && stringValue.Length >= assignmentField.Min;
                        default:
                            return true;
                    }
                }
                return true;
            })
            .WithMessage("Delivery field text length must be between min and max");

        RuleForEach(x => x.Fields)
            .Must((delivery, field) =>
            {
                var assignmentField = delivery.Assignment!.Fields!.FirstOrDefault(f => f.Id == field.AssignmentFieldId);
                if (assignmentField?.Max is not null)
                {
                    switch (assignmentField.Type)
                    {
                        case AssignmentDataType.Integer:
                            var intValue = field.JsonValue?.Deserialize<int>();
                            return intValue is not null && intValue.Value <= assignmentField.Max;
                        case AssignmentDataType.Float:
                            var floatValue = field.JsonValue?.Deserialize<double>();
                            return floatValue is not null && floatValue.Value <= assignmentField.Max;
                        default:
                            return true;
                    }
                }
                return true;
            })
            .WithMessage("Delivery field value must be between min and max");

        RuleForEach(x => x.Fields)
            .Must((delivery, field) =>
            {
                var assignmentField = delivery.Assignment!.Fields!.FirstOrDefault(f => f.Id == field.AssignmentFieldId);
                if (assignmentField?.Max is not null)
                {
                    switch (assignmentField.Type)
                    {
                        case AssignmentDataType.ShortText:
                        case AssignmentDataType.LongText:
                            var stringValue = field.JsonValue?.Deserialize<string>();
                            return stringValue is not null && stringValue.Length <= assignmentField.Min;
                        default:
                            return true;
                    }
                }
                return true;
            })
            .WithMessage("Delivery field text length must be between min and max");

        RuleForEach(x => x.Fields)
            .Must((delivery, field) =>
            {
                var assignmentField = delivery.Assignment!.Fields!.FirstOrDefault(f => f.Id == field.AssignmentFieldId);
                if (assignmentField?.Regex is not null)
                {
                    switch (assignmentField.Type)
                    {
                        case AssignmentDataType.ShortText:
                        case AssignmentDataType.LongText:
                            var stringValue = field.JsonValue?.Deserialize<string>();
                            return stringValue is not null && Regex.IsMatch(stringValue, assignmentField.Regex);
                        default:
                            return true;
                    }
                }
                return true;
            })
            .WithMessage("Delivery field text must match regex");
    }
}
