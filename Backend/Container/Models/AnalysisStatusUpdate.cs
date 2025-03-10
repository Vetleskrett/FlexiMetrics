namespace Container.Models;

public record AnalysisStatusUpdate(Guid AnalysisId, Guid AnalysisEntryId, string Logs);
