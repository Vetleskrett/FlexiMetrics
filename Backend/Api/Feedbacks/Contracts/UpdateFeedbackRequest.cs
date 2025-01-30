using Database.Models;

namespace Api.Feedbacks.Contracts;

public class UpdateFeedbackRequest
{
    public required string Comment { get; init; }
    public GradingRequest? Grading { get; init; }
    public required Guid DeliveryId { get; init; }
}
