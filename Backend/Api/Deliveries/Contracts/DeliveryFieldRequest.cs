namespace Api.Deliveries.Contracts;

public class DeliveryFieldRequest
{
    public required Guid AssignmentFieldId { get; init; }
    public required object Value { get; init; }
}