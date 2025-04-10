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
            FileName = analyzer.FileName
        };
    }

    public static List<AnalyzerResponse> MapToResponse(this IEnumerable<Analyzer> analyzers)
    {
        return analyzers.Select(analyzer => analyzer.MapToResponse()).ToList();
    }
}
