using Database.Models;

namespace Api.Feedbacks.Contracts;

public class CreateFeedbackRequest
{
    public required Guid AssignmentId { get; init; }
    public Guid? StudentId { get; init; }
    public Guid? TeamId { get; init; }
    public required string Comment { get; init; }
    public bool? IsApproved { get; init; }
    public LetterGrade? LetterGrade { get; init; }
    public int? Points { get; init; }
}