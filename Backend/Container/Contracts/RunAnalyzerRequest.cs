namespace Container.Contracts;

public record RunAnalyzerRequest(Guid CourseId, Guid AssignmentId, Guid AnalyzerId, Guid AnalysisId);
