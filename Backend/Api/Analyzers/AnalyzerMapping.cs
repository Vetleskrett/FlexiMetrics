using Api.Analyzers.Contracts;
using Database.Models;

namespace Api.Analyzers;

public static class AnalyzerMapping
{
    public static Analyzer MapToAnalyzer(this CreateAnalyzerRequest request)
    {
        var id = Guid.NewGuid();
        return new Analyzer
        {
            Id = id,
            AssignmentId = request.AssignmentId,
            Name = request.Name,
            Requirements = request.Requirements,
            AptPackages = request.AptPackages,
            State = AnalyzerState.Building,
            FileName = request.FileName
        };
    }

    public static AnalyzerResponse MapToResponse(this Analyzer analyzer)
    {
        return new AnalyzerResponse
        {
            Id = analyzer.Id,
            AssignmentId = analyzer.AssignmentId,
            Name = analyzer.Name,
            Requirements = analyzer.Requirements,
            AptPackages = analyzer.AptPackages,
            State = analyzer.State,
            FileName = analyzer.FileName
        };
    }

    public static List<AnalyzerResponse> MapToResponse(this IEnumerable<Analyzer> analyzers)
    {
        return analyzers.Select(analyzer => analyzer.MapToResponse()).ToList();
    }

    public static AnalyzerLogResponse MapToResponse(this AnalyzerLog analyzerlog)
    {
        return new AnalyzerLogResponse
        {
            Id = analyzerlog.Id,
            Timestamp = analyzerlog.Timestamp,
            Type = analyzerlog.Type,
            Category = analyzerlog.Category,
            Text = analyzerlog.Text
        };
    }

    public static List<AnalyzerLogResponse> MapToResponse(this IEnumerable<AnalyzerLog> analyzers)
    {
        return analyzers.Select(analyzer => analyzer.MapToResponse()).ToList();
    }
}
