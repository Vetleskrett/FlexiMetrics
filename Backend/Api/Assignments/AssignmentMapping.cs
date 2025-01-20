using Api.AssignmentFields;
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
            CourseId = request.CourseId,
            Fields = request.Fields.MapToAssignmentField().ToList(),
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
            CourseId = assignment.CourseId,
        };
    }

    public static IEnumerable<AssignmentResponse> MapToResponse(this IEnumerable<Assignment> assignments)
    {
        return assignments.Select(assignment => assignment.MapToResponse());
    }
}
