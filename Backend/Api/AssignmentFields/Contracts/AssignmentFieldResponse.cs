using Database.Models;

namespace Api.AssignmentFields.Contracts;

public class AssignmentFieldResponse
{
    public required Guid Id { get; set; }
    public required AssignmentDataType Type { get; set; }
    public required string Name { get; set; }
    public required Guid AssignmentId { get; set; }
}