namespace Api.AssignmentFields.Contracts;

public class CreateAssignmentFieldsRequest
{
    public required List<CreateAssignmentFieldRequest> Fields { get; init; }
}
