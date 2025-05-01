namespace Api.Analyzers.Contracts;

public class CreateAnalyzerRequest
{
    public required string Name { get; init; }
    public required string Requirements { get; init; }
    public required string AptPackages { get; init; }
    public required string FileName { get; init; }
    public required Guid AssignmentId { get; init; }
}