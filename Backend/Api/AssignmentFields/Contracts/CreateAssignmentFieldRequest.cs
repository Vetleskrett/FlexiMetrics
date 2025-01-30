using Database.Models;

namespace Api.AssignmentFields.Contracts;

public class CreateAssignmentFieldRequest
{
    public required AssignmentDataType Type { get; init; }
    public required string Name { get; init; }
    public required Guid AssignmentId { get; init; }
}