using Api.Analyses.Contracts;

namespace Api.Analyzers.Contracts;

public class AnalyzerStatusUpdateResponse
{
	public required AnalysisResponse? Analysis { get; init; }
	public required string Logs { get; init; }
}