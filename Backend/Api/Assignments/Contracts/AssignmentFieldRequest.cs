using Database.Models;

namespace Api.Assignments.Contracts;

public class AssignmentFieldRequest
{
    public required AssignmentDataType Type { get; set; }
    public required string Name { get; set; }
    public required Guid AssignmentId { get; set; }
}