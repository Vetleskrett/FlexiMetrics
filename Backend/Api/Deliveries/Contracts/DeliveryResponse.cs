namespace Api.Deliveries.Contracts;

public class DeliveryResponse
{
    public required Guid Id { get; init; }
    public required Guid AssignmentId { get; init; }
    public required Guid? StudentId { get; init; }
    public required Guid? TeamId { get; init; }
    public required List<DeliveryFieldResponse> Fields { get; init; }
}

public class DeliveryFieldResponse
{
    public required Guid Id { get; init; }
    public required Guid AssignmentFieldId { get; init; }
    public required string Value { get; init; }
}