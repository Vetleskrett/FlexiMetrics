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
            DeliveryAnalyses = analysis.DeliveryAnalyses!.MapToResponse()
        };
    }

    public static List<AnalysisResponse> MapToResponse(this IEnumerable<Analysis> analyses)
    {
        return analyses.Select(analysis => analysis.MapToResponse()).ToList();
    }

    public static DeliveryAnalysisResponse MapToResponse(this DeliveryAnalysis deliveryAnalysis)
    {
        return new DeliveryAnalysisResponse
        {
            Id = deliveryAnalysis.Id,
            AnalysisId = deliveryAnalysis.AnalysisId,
            DeliveryId = deliveryAnalysis.DeliveryId,
            Team = deliveryAnalysis.Delivery?.Team?.MapToResponse(),
            Student = deliveryAnalysis.Delivery?.Student?.MapToStudentResponse(),
            Fields = deliveryAnalysis.Fields!.MapToResponse()
        };
    }

    public static List<DeliveryAnalysisResponse> MapToResponse(this IEnumerable<DeliveryAnalysis> deliveryAnalyses)
    {
        return deliveryAnalyses.Select(deliveryAnalysis => deliveryAnalysis.MapToResponse()).ToList();
    }

    public static DeliveryAnalysisFieldResponse MapToResponse(this DeliveryAnalysisField deliveryAnalysisField)
    {
        return new DeliveryAnalysisFieldResponse
        {
            Id = deliveryAnalysisField.Id,
            Name = deliveryAnalysisField.Name,
            Type = deliveryAnalysisField.Type,
            Value = deliveryAnalysisField.Value
        };
    }

    public static List<DeliveryAnalysisFieldResponse> MapToResponse(this IEnumerable<DeliveryAnalysisField> deliveryAnalysisFields)
    {
        return deliveryAnalysisFields.Select(deliveryAnalysisField => deliveryAnalysisField.MapToResponse()).ToList();
    }
}
