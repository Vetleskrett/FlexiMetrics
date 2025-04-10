using Api.Analyses.Contracts;

namespace Api.Analyzers.Contracts;

public class AnalyzerStatusUpdateResponse
{
    public required AnalyzerResponse Analyzer { get; init; }
    public required AnalysisResponse? Analysis { get; init; }
}
