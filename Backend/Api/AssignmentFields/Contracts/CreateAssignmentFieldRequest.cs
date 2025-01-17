using Database.Models;

namespace Api.AssignmentFields.Contracts;

public class CreateAssignmentFieldRequest
{
    public required AssignmentDataType Type { get; set; }
    public required string Name { get; set; }
    public required Guid AssignmentId { get; set; }
}