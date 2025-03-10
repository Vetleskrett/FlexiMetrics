namespace Container.Models;

public record AnalysisStatusUpdate(Guid AnalysisId, Guid DeliveryAnalysisId, string Logs);
