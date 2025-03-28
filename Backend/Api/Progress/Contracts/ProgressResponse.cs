namespace Api.Progress.Contracts;

public class ProgressResponse
{
    public required Guid Id { get; init; }
    public required List<AssignmentProgressResponse> AssignmentsProgress { get; init; }
}