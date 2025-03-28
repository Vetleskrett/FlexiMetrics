namespace Api.Progress.Contracts;

public class ProgressResponse
{
    public required Guid Id { get; init; }
    public required List<AssignmentProgressResponse> AssignmentsProgress { get; init; }
}

public class AssignmentProgressResponse
{
    public required Guid Id { get; init; }
    public required bool IsDelivered { get; init; }
}
