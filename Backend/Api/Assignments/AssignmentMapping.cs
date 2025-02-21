using Api.Assignments.Contracts;
using Database.Models;

namespace Api.Assignments;

public static class AssignmentMapping
{
    public static Assignment MapToAssignment(this CreateAssignmentRequest request)
    {
        var id = Guid.NewGuid();
        return new Assignment
        {
            Id = id,
            DueDate = request.DueDate,
            Name = request.Name,
            Published = request.Published,
            CollaborationType = request.CollaborationType,
            Mandatory = request.Mandatory,
            GradingType = request.GradingType,
            MaxPoints = request.MaxPoints,
            Description = request.Description,
            CourseId = request.CourseId,
            Fields = request.Fields.MapToAssignmentField(id).ToList()
        };
    }

    public static List<AssignmentField> MapToAssignmentField(this IEnumerable<CreateAssignmentFieldRequest> fields, Guid assignmentId)
    {
        return fields.Select(field => field.MapToAssignmentField(assignmentId)).ToList();
    }

    public static AssignmentField MapToAssignmentField(this CreateAssignmentFieldRequest request, Guid assignmentId)
    {
        return new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            Type = request.Type,
            Name = request.Name,
            Min = request.Min,
            Max = request.Max,
            Regex = request.Regex,
            SubType = request.SubType,
        };
    }

    public static Assignment MapToAssignment(this UpdateAssignmentRequest request, Guid id, Guid courseId)
    {
        return new Assignment
        {
            Id = id,
            DueDate = request.DueDate,
            Name = request.Name,
            Published = request.Published,
            CollaborationType = request.CollaborationType,
            Mandatory = request.Mandatory,
            GradingType = request.GradingType,
            MaxPoints = request.MaxPoints,
            Description = request.Description,
            CourseId = courseId,
        };
    }

    public static AssignmentResponse MapToResponse(this Assignment assignment)
    {
        return new AssignmentResponse
        {
            Id = assignment.Id,
            Name = assignment.Name,
            DueDate = assignment.DueDate,
            Published = assignment.Published,
            CollaborationType = assignment.CollaborationType,
            Mandatory = assignment.Mandatory,
            GradingType = assignment.GradingType,
            MaxPoints = assignment.MaxPoints,
            Description = assignment.Description,
            CourseId = assignment.CourseId,
        };
    }

    public static List<AssignmentResponse> MapToResponse(this IEnumerable<Assignment> assignments)
    {
        return assignments.Select(assignment => assignment.MapToResponse()).ToList();
    }
}
