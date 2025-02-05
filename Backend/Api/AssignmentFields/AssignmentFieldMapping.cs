using Api.AssignmentFields.Contracts;
using Api.Assignments.Contracts;
using Database.Models;

namespace Api.AssignmentFields;

public static class AssignmentFieldMapping
{
    public static AssignmentField MapToAssignmentField(this CreateAssignmentFieldRequest request)
    {
        return new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = request.AssignmentId,
            Type = request.Type,
            Name = request.Name,
        };
    }

    public static AssignmentField MapToNewAssignmentField(this NewAssignmentFieldRequest request, Guid assignmentId)
    {
        return new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            Type = request.Type,
            Name = request.Name,
        };
    }

    public static IEnumerable<AssignmentField> MapToAssignmentField(this IEnumerable<CreateAssignmentFieldRequest> fields)
    {
        return fields.Select(field => field.MapToAssignmentField());
    }

    public static IEnumerable<AssignmentField> MapToNewAssignmentField(this IEnumerable<NewAssignmentFieldRequest> fields, Guid assignmentId)
    {
        return fields.Select(field => field.MapToNewAssignmentField(assignmentId));
    }

    public static AssignmentField MapToAssignmentField(this UpdateAssignmentFieldRequest request, Guid id, Guid assignmentId)
    {
        return new AssignmentField
        {
            Id = id,
            AssignmentId = assignmentId,
            Type = request.Type,
            Name = request.Name,
        };
    }

    public static IEnumerable<AssignmentField> MapToAssignmentField(this IEnumerable<UpdateAssignmentFieldRequest> fields, Guid id, Guid assignmentId)
    {
        return fields.Select(field => field.MapToAssignmentField(id, assignmentId));
    }

    public static AssignmentFieldResponse MapToResponse(this AssignmentField field)
    {
        return new AssignmentFieldResponse
        {
            Id = field.Id,
            Name = field.Name,
            Type = field.Type,
            AssignmentId = field.AssignmentId,
        };
    }

    public static IEnumerable<AssignmentFieldResponse> MapToResponse(this IEnumerable<AssignmentField> fields)
    {
        return fields.Select(field => field.MapToResponse());
    }
}
