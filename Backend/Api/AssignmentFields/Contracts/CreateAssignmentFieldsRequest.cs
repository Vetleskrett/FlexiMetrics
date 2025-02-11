using Database.Models;

namespace Api.AssignmentFields.Contracts
{
    public class CreateAssignmentFieldsRequest
    {
        public required List<CreateAssignmentFieldRequest> fields { get; init; }
    }
}
