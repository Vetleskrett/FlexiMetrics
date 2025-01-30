using Database.Models;

namespace Api.AssignmentFields.Contracts;

public class UpdateAssignmentFieldRequest
{
    public required AssignmentDataType Type { get; init; }
    public required string Name { get; init; }
}