using Database.Models;

namespace Api.Feedbacks.Contracts;

public class UpdateFeedbackRequest
{
    public required string Comment { get; init; }
    public bool? IsApproved { get; init; }
    public LetterGrade? LetterGrade { get; init; }
    public int? Points { get; init; }
}
