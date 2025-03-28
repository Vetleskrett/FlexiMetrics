namespace Api.Progress.Contracts;

public class AssignmentProgressResponse
{
    public required Guid Id { get; init; }
    public required bool IsDelivered { get; init; }
}