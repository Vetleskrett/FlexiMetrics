using Api.Analyses.Contracts;

namespace Api.Analyses;

public static class AnalysisEndpoints
{
    public static void MapAnalysisEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Analyses").RequireAuthorization();

        group.MapGet("analyses", async (IAnalysisService analysisService) =>
        {
            var result = await analysisService.GetAll();
            return result.MapToResponse(analyses => Results.Ok(analyses));
        })
        .Produces<IEnumerable<AnalysisResponse>>()
        .WithName("GetAllAnalyses")
        .WithSummary("Get all analyses");

        group.MapGet("analyses/{id:guid}", async (IAnalysisService analysisService, Guid id) =>
        {
            var result = await analysisService.GetById(id);
            return result.MapToResponse(analysis => Results.Ok(analysis));
        })
        .Produces<AnalysisResponse>()
        .WithName("GetAnalysis")
        .WithSummary("Get analysis by id");

        group.MapGet("analyzers/{analyzerId:guid}/analyses", async (IAnalysisService analysisService, Guid analyzerId) =>
        {
            var result = await analysisService.GetAllByAnalyzer(analyzerId);
            return result.MapToResponse(analyses => Results.Ok(analyses));
        })
        .Produces<AnalyzerAnalysesResponse>()
        .WithName("GetAllAnalysesByAnalyzer")
        .WithSummary("Get all analyses by analyzer id");

        group.MapDelete("analyses/{id:guid}", async (IAnalysisService analysisService, Guid id) =>
        {
            var result = await analysisService.DeleteById(id);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteAnalysis")
        .WithSummary("Delete analysis by id");
    }
}
