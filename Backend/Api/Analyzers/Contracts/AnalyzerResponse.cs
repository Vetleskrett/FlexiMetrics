namespace Api.Analyzers.Contracts;

public class AnalyzerResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Requirements { get; init; }
    public required string AptPackages { get; init; }
    public required string FileName { get; init; }
    public required Guid AssignmentId { get; init; }
}