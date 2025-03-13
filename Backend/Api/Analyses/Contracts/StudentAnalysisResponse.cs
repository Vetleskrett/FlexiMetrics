namespace Api.Analyses.Contracts;

public class StudentAnalysisResponse
{
    public required Guid Id { get; init; }
    public required string AnalyzerName { get; init; }
    public required DateTime CompletedAt { get; init; }
    public required List<AnalysisFieldResponse> Fields { get; init; }
}
