using Database.Models;

namespace Api.Analyses.Contracts;

public class AnalysisFieldResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required AnalysisFieldType Type { get; init; }
    public required object Value { get; init; }
}