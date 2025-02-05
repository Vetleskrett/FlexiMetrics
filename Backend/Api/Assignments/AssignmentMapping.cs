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
            GradingFormat = request.GradingFormat.MapToGradingFormat(),
            Description = request.Description,
            CourseId = request.CourseId,
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
            GradingFormat = request.GradingFormat.MapToGradingFormat(),
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
            GradingFormat = assignment.GradingFormat.MapToResponse(),
            Description = assignment.Description,
            CourseId = assignment.CourseId,
        };
    }

    public static IEnumerable<AssignmentResponse> MapToResponse(this IEnumerable<Assignment> assignments)
    {
        return assignments.Select(assignment => assignment.MapToResponse());
    }

    public static GradingFormat MapToGradingFormat(this GradingFormatRequest request)
    {
        return new GradingFormat
        {
            GradingType = request.GradingType,
            MaxPoints = request.MaxPoints,
        };
    }

    public static GradingFormatResponse MapToResponse(this GradingFormat gradingFormat)
    {
        return new GradingFormatResponse
        {
            GradingType = gradingFormat.GradingType,
            MaxPoints = gradingFormat.MaxPoints,
        };
    }
}
