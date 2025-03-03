using Database.Models;

namespace Container;

public record RunAnalyzerRequest(Guid CourseId, Guid AssignmentId, Guid AnalyzerId);
