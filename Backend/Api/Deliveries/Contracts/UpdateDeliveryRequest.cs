namespace Api.Deliveries.Contracts;

public class UpdateDeliveryRequest
{
    public required List<DeliveryFieldRequest> Fields { get; init; }
}
