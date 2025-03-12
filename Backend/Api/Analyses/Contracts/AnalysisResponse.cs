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
    public required List<AnalysisEntryResponse> AnalysisEntries { get; init; }
    public required int TotalNumEntries { get; init; }
}

public class AnalysisEntryResponse
{
    public required Guid Id { get; init; }
    public required Guid AnalysisId { get; init; }
    public required TeamResponse? Team { get; init; }
    public required StudentResponse? Student { get; init; }
    public required List<AnalysisFieldResponse> Fields { get; init; }
    public required string LogInformation { get; init; }
    public required string LogError { get; init; }
    public required DateTime CompletedAt { get; init; }
}

public class AnalysisFieldResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required AnalysisFieldType Type { get; init; }
    public required object Value { get; init; }
}