using Api.Students.Contracts;
using Api.Teams.Contracts;
using Database.Models;

namespace Api.Analyses.Contracts;

public class AnalysisResponse
{
    public required Guid Id { get; init; }
    public required DateTime StartedAt { get; init; }
    public required DateTime? CompletedAt { get; init; }
    public required AnalysisStatus Status { get; init; }
    public required Guid AnalyzerId { get; init; }
    public required List<DeliveryAnalysisResponse> DeliveryAnalyses { get; init; }
}

public class DeliveryAnalysisResponse
{
    public required Guid Id { get; init; }
    public required Guid AnalysisId { get; init; }
    public required Guid DeliveryId { get; init; }
    public required TeamResponse? Team { get; init; }
    public required StudentResponse? Student { get; init; }
    public required List<DeliveryAnalysisFieldResponse> Fields { get; init; }
}

public class DeliveryAnalysisFieldResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required AnalysisFieldType Type { get; init; }
    public required object Value { get; init; }
}