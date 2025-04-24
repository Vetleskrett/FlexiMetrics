using Database.Models;

namespace Api.Analyzers.Contracts;

public class AnalyzerLogResponse
{
    public required Guid Id { get; init; }
    public required DateTime Timestamp { get; init; }
    public required AnalyzerLogType Type { get; init; }
    public required string Category { get; init; }
    public required string Text { get; init; }
}