namespace Api.Progress.Contracts;

public class SlimProgressResponse
{
    public required Guid Id { get; init; }
    public required List<SlimAssignmentProgressResponse> AssignmentsProgress { get; init; }
}

public class SlimAssignmentProgressResponse
{
    public required Guid Id { get; init; }
    public required bool IsDelivered { get; init; }
}