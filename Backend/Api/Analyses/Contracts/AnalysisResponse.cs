﻿using Api.Students.Contracts;
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
}

public class AnalysisEntryResponse
{
    public required Guid Id { get; init; }
    public required Guid AnalysisId { get; init; }
    public required TeamResponse? Team { get; init; }
    public required StudentResponse? Student { get; init; }
    public required List<AnalysisFieldResponse> Fields { get; init; }
    public required DateTime? CompletedAt { get; init; }
}