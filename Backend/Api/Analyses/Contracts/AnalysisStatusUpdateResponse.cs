namespace Api.Analyses.Contracts;

public class AnalysisStatusUpdateResponse
{
    public required DeliveryAnalysisResponse? DeliveryAnalysis { get; init; }
    public required string Logs { get; init; }
}