using Api.AssignmentFields.Contracts;
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
    public required GradingFormatRequest GradingFormat { get; init; }
    public required string Description { get; init; }
    public required List<NewAssignmentFieldRequest> Fields { get; init; }
}

public class NewAssignmentFieldRequest
{
    public required AssignmentDataType Type { get; init; }
    public required string Name { get; init; }
}