namespace Api.Deliveries.Contracts;

public class CreateDeliveryRequest
{
    public required Guid AssignmentId { get; init; }
    public required Guid StudentId { get; init; }
    public required List<DeliveryFieldRequest> Fields { get; init; }
}