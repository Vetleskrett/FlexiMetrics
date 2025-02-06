using Database.Models;

namespace Api.Assignments.Contracts;

public class AssignmentResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required DateTime DueDate { get; init; }
    public required bool Published { get; init; }
    public required CollaborationType CollaborationType { get; init; }
    public required bool Mandatory { get; init; }
    public required GradingType GradingType { get; init; }
    public required int? MaxPoints { get; init; }
    public required string Description { get; init; }
    public required Guid CourseId { get; init; }
}