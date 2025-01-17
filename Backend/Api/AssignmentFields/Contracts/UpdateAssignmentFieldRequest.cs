using Database.Models;

namespace Api.AssignmentFields.Contracts;

public class UpdateAssignmentFieldRequest
{
    public required AssignmentDataType Type { get; set; }
    public required string Name { get; set; }
}