using Api.Analyses.Contracts;
using Api.Teams;
using Api.Students;
using Database.Models;

namespace Api.Analyses;

public static class AnalysisMapping
{
    public static SlimAnalysisResponse MapToSlimResponse(this Analysis analysis)
    {
        return new SlimAnalysisResponse
        {
            Id = analysis.Id,
            StartedAt = analysis.StartedAt,
            CompletedAt = analysis.CompletedAt,
            Status = analysis.Status,
            AnalyzerId = analysis.AnalyzerId,
        };
    }

    public static List<SlimAnalysisResponse> MapToSlimResponse(this IEnumerable<Analysis> analyses)
    {
        return analyses.Select(analysis => analysis.MapToSlimResponse()).ToList();
    }

    public static AnalysisResponse MapToResponse(this Analysis analysis)
    {
        return new AnalysisResponse
        {
            Id = analysis.Id,
            StartedAt = analysis.StartedAt,
            CompletedAt = analysis.CompletedAt,
            Status = analysis.Status,
            AnalyzerId = analysis.AnalyzerId,
            AnalysisEntries = analysis.AnalysisEntries!.MapToResponse(),
            TotalNumEntries = analysis.TotalNumEntries
        };
    }

    public static List<AnalysisResponse> MapToResponse(this IEnumerable<Analysis> analyses)
    {
        return analyses.Select(analysis => analysis.MapToResponse()).ToList();
    }

    public static AnalysisEntryResponse MapToResponse(this AnalysisEntry analysisEntry)
    {
        return new AnalysisEntryResponse
        {
            Id = analysisEntry.Id,
            AnalysisId = analysisEntry.AnalysisId,
            Team = analysisEntry.Team?.MapToResponse(),
            Student = analysisEntry.Student?.MapToStudentResponse(),
            Fields = analysisEntry.Fields!.MapToResponse(),
            LogInformation = analysisEntry.LogInformation,
            LogError = analysisEntry.LogError,
            CompletedAt = analysisEntry.CompletedAt,
        };
    }

    public static List<AnalysisEntryResponse> MapToResponse(this IEnumerable<AnalysisEntry> analysisEntries)
    {
        return analysisEntries.Select(analysisEntry => analysisEntry.MapToResponse()).ToList();
    }

    public static StudentAnalysisResponse MapToStudentResponse(this AnalysisEntry analysisEntry)
    {
        return new StudentAnalysisResponse
        {
            Id = analysisEntry.Id,
            AnalyzerName = analysisEntry.Analysis!.Analyzer!.Name,
            Fields = analysisEntry.Fields!.MapToResponse(),
            CompletedAt = analysisEntry.CompletedAt,
        };
    }

    public static List<StudentAnalysisResponse> MapToStudentResponse(this IEnumerable<AnalysisEntry> analysisEntries)
    {
        return analysisEntries.Select(analysisEntry => analysisEntry.MapToStudentResponse()).ToList();
    }

    public static AnalysisFieldResponse MapToResponse(this AnalysisField analysisField)
    {
        return new AnalysisFieldResponse
        {
            Id = analysisField.Id,
            Name = analysisField.Name,
            Type = analysisField.Type,
            Value = analysisField.Value
        };
    }

    public static List<AnalysisFieldResponse> MapToResponse(this IEnumerable<AnalysisField> analysisFields)
    {
        return analysisFields.Select(analysisField => analysisField.MapToResponse()).ToList();
    }
}
