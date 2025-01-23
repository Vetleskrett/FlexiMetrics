namespace Api.Deliveries.Contracts;

public class UpdateDeliveryRequest
{
    public required List<UpdateDeliveryFieldRequest> Fields { get; init; }
}

public class UpdateDeliveryFieldRequest
{
    public required Guid Id { get; init; }
    public required object Value { get; init; }
}