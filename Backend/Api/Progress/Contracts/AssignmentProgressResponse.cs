using Api.Assignments.Contracts;
using Api.Feedbacks.Contracts;

namespace Api.Progress.Contracts;

public class AssignmentProgressResponse
{
    public required AssignmentResponse Assignment { get; init; }
    public required FeedbackResponse? Feedback { get; init; }
    public required Guid? StudentId { get; init; }
    public required Guid? TeamId { get; init; }
    public required bool IsDelivered { get; init; }
}