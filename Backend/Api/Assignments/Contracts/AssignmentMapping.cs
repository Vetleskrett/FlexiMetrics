using Database.Models;

namespace Api.Assignments.Contracts;

public static class AssignmentMapping
{
    public static Assignment MapToAssignment(this CreateAssignmentRequest request)
    {
        return new Assignment
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            DueDate = request.DueDate,
            Name = request.Name,
        };
    }

    public static Assignment MapToAssignment(this UpdateAssignmentRequest request, Guid id)
    {
        return new Assignment
        {
            Id = id,
            CourseId = request.CourseId,
            DueDate = request.DueDate,
            Name = request.Name,
        };
    }

    public static AssignmentResponse MapToResponse(this Assignment assignment)
    {
        return new AssignmentResponse
        {
            Id = assignment.Id,
            Name = assignment.Name,
            DueDate = assignment.DueDate,
            CourseId = assignment.CourseId,
        };
    }

    public static IEnumerable<AssignmentResponse> MapToResponse(this IEnumerable<Assignment> assignments)
    {
        return assignments.Select(assignment => assignment.MapToResponse());
    }


    public static AssignmentResponse MapToResponse(this Assignment assignment, IEnumerable<AssignmentField> assignmentFields)
    {
        return new AssignmentResponse
        {
            Id = assignment.Id,
            Name = assignment.Name,
            DueDate = assignment.DueDate,
            CourseId = assignment.CourseId,
            Fields = assignmentFields.MapToResponse().ToList(),
        };
    }

    public static AssignmentField MapToAssignmentField(this AssignmentFieldRequest request)
    {
        return new AssignmentField
        {
            Id = Guid.NewGuid(),
            Type = request.Type,
            Name = request.Name,
            AssignmentId = request.AssignmentId,
        };
    }

    public static IEnumerable<AssignmentField> MapToAssignmentField(this IEnumerable<AssignmentFieldRequest> fields)
    {
        return fields.Select(field => field.MapToAssignmentField());
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
