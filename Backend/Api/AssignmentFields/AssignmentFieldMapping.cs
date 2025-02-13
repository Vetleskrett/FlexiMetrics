﻿using Api.AssignmentFields.Contracts;
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

    public static List<AssignmentField> MapToAssignmentField(this IEnumerable<CreateAssignmentFieldRequest> fields)
    {
        return fields.Select(field => field.MapToAssignmentField()).ToList();
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

    public static List<AssignmentField> MapToAssignmentField(this IEnumerable<UpdateAssignmentFieldRequest> fields, Guid id, Guid assignmentId)
    {
        return fields.Select(field => field.MapToAssignmentField(id, assignmentId)).ToList();
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

    public static List<AssignmentFieldResponse> MapToResponse(this IEnumerable<AssignmentField> fields)
    {
        return fields.Select(field => field.MapToResponse()).ToList();
    }
}
