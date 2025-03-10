namespace Api.Analyses.Contracts;

public class AnalysisStatusUpdateResponse
{
	public required AnalysisEntryResponse? AnalysisEntry { get; init; }
	public required string Logs { get; init; }
}