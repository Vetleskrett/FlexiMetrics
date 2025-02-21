using Database.Models;

namespace Api.Assignments.Contracts;

public class CreateAssignmentRequest
{
    public required string Name { get; init; }
    public required Guid CourseId { get; init; }
    public required DateTime DueDate { get; init; }
    public required bool Published { get; init; }
    public required CollaborationType CollaborationType { get; init; }
    public required bool Mandatory { get; init; }
    public required GradingType GradingType { get; init; }
    public int? MaxPoints { get; init; }
    public required string Description { get; init; }
    public required List<CreateAssignmentFieldRequest> Fields { get; init; }
}

public class CreateAssignmentFieldRequest
{
    public required AssignmentDataType Type { get; init; }
    public required string Name { get; init; }

    public int? Min { get; init; }
    public int? Max { get; init; }
    public string? Regex { get; init; }
    public AssignmentDataType? SubType { get; init; }
}