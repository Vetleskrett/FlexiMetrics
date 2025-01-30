using Database.Models;

namespace Api.Feedbacks.Contracts;

public class FeedbackResponse
{
    public required Guid Id { get; init; }
    public required string Comment { get; init; }
    public required Guid DeliveryId { get; init; }

    public bool? IsApproved { get; init; }
    public LetterGrade? LetterGrade { get; init; }
    public int? Points { get; init; }
}