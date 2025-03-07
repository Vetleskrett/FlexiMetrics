using Api.Analyses.Contracts;
using System.Text.Json;

namespace Api.Analyses;

public static class AnalysisEndpoints
{
    public static void MapAnalysisEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Analyses");

        group.MapGet("analyses", async (IAnalysisService analysisService) =>
        {
            var result = await analysisService.GetAll();
            return result.MapToResponse(analyses => Results.Ok(analyses));
        })
        .Produces<IEnumerable<SlimAnalysisResponse>>()
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

        group.MapGet("analyses/{id:guid}/status", async (HttpContext context, IAnalysisService analysisService, Guid id) =>
        {
            context.Response.Headers.Append("Content-Type", "text/event-stream");
            await foreach (var statusUpdate in analysisService.GetStatusEventsById(id, context.RequestAborted))
            {
                var json = JsonSerializer.Serialize(statusUpdate, JsonSerializerOptions.Web);
                await context.Response.WriteAsync($"data: {json}\n\n");
                await context.Response.Body.FlushAsync();
            }
        })
        .Produces(StatusCodes.Status200OK, contentType: "text/event-stream")
        .WithName("GetAnalysisStatus")
        .WithSummary("Get analysis status by id");

        group.MapDelete("analyses/{id:guid}", async (IAnalysisService analysisService, Guid id) =>
        {
            var result = await analysisService.DeleteById(id);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteAnalysis")
        .WithSummary("Delete analysis by id");
    }
}
