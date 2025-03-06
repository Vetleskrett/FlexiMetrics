using Database.Models;

namespace Api.Analyses.Contracts;

public class AnalyzerAnalysesResponse
{
    public required List<SlimAnalysisResponse> Analyses { get; init; }
    public required AnalysisResponse? Latest { get; init; }
}

public class SlimAnalysisResponse
{
    public required Guid Id { get; init; }
    public required DateTime StartedAt { get; init; }
    public required DateTime? CompletedAt { get; init; }
    public required AnalysisStatus Status { get; init; }
    public required Guid AnalyzerId { get; init; }
}