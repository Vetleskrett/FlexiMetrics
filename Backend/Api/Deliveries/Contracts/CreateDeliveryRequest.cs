namespace Api.Deliveries.Contracts;

public class CreateDeliveryRequest
{
    public required Guid AssignmentId { get; init; }
    public required Guid StudentId { get; init; }
    public required List<CreateDeliveryFieldRequest> Fields { get; init; }
}

public class CreateDeliveryFieldRequest
{
    public required Guid AssignmentFieldId { get; init; }
    public required object Value { get; init; }
}