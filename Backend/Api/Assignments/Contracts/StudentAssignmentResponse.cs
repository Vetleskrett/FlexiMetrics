using Database.Models;

namespace Api.Assignments.Contracts;

public class StudentAssignmentResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required DateTime DueDate { get; init; }
    public required CollaborationType CollaborationType { get; init; }
    public required bool IsDelivered { get; init; }
    public required Guid CourseId { get; init; }
}
