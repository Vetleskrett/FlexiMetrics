using Api.AssignmentFields.Contracts;
using Database.Models;

namespace Api.AssignmentFields;

public static class AssignmentFieldMapping
{
    public static AssignmentField MapToAssignmentField(this AssignmentFieldRequest request, Guid assignmentId)
    {
        return new AssignmentField
        {
            Id = request.Id.HasValue ? request.Id.Value : Guid.NewGuid(),
            AssignmentId = assignmentId,
            Type = request.Type,
            Name = request.Name,
            RangeMin = request.RangeMin,
            RangeMax = request.RangeMax
        };
    }

    public static List<AssignmentField> MapToAssignmentField(this IEnumerable<AssignmentFieldRequest> fields, Guid assignmentId)
    {
        return fields.Select(field => field.MapToAssignmentField(assignmentId)).ToList();
    }

    public static AssignmentField MapToAssignmentField(this AssignmentFieldRequest request, Guid id, Guid assignmentId)
    {
        return new AssignmentField
        {
            Id = id,
            AssignmentId = assignmentId,
            Type = request.Type,
            Name = request.Name,
            RangeMin = request.RangeMin,
            RangeMax = request.RangeMax
        };
    }

    public static AssignmentFieldResponse MapToResponse(this AssignmentField field)
    {
        return new AssignmentFieldResponse
        {
            Id = field.Id,
            Name = field.Name,
            Type = field.Type,
            AssignmentId = field.AssignmentId,
            RangeMin = field.RangeMin,
            RangeMax = field.RangeMax
        };
    }

    public static List<AssignmentFieldResponse> MapToResponse(this IEnumerable<AssignmentField> fields)
    {
        return fields.Select(field => field.MapToResponse()).ToList();
    }
}
