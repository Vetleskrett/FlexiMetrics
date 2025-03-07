namespace Container;

public record AnalysisStatusUpdate(Guid AnalysisId, Guid DeliveryAnalysisId, string Logs);
