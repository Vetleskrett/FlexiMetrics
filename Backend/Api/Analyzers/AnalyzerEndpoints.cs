using Api.Analyzers.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Analyzers;

public static class AnalyzerEndpoints
{
    public static void MapAnalyzerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Analyzers");

        group.MapGet("analyzers", async (IAnalyzerService analyzerService) =>
        {
            var result = await analyzerService.GetAll();
            return result.MapToResponse(analyzers => Results.Ok(analyzers));
        })
        .Produces<IEnumerable<AnalyzerResponse>>()
        .WithName("GetAllAnalyzers")
        .WithSummary("Get all analyzers");

        group.MapGet("analyzers/{analyzerId:guid}", async (IAnalyzerService analyzerService, Guid analyzerId) =>
        {
            var result = await analyzerService.GetById(analyzerId);
            return result.MapToResponse(analyzer => Results.Ok(analyzer));
        })
        .Produces<AnalyzerResponse>()
        .WithName("GetAnalyzer")
        .WithSummary("Get analyzer by id");

        group.MapPost("analyzers/{analyzerId:guid}/action", async (IAnalyzerService analyzerService, Guid analyzerId, AnalyzerActionRequest request) =>
        {
            var result = await analyzerService.StartAction(request, analyzerId);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("StartAnalyzerAction")
        .WithSummary("Start analyzer action by id");

        group.MapGet("assignments/{assignmentId:guid}/analyzers", async (IAnalyzerService analyzerService, Guid assignmentId) =>
        {
            var result = await analyzerService.GetAllByAssignment(assignmentId);
            return result.MapToResponse(analyzers => Results.Ok(analyzers));
        })
        .Produces<IEnumerable<AnalyzerResponse>>()
        .WithName("GetAllAnalyzersByAssignment")
        .WithSummary("Get all analyzers by assignment id");

        group.MapPost("analyzers", async (IAnalyzerService analyzerService, CreateAnalyzerRequest request) =>
        {
            var result = await analyzerService.Create(request);
            return result.MapToResponse(analyzer => Results.CreatedAtRoute
            (
                "GetAnalyzer",
                new { analyzerId = analyzer.Id },
                analyzer
            ));
        })
        .Produces<AnalyzerResponse>()
        .WithName("CreateAnalyzer")
        .WithSummary("Create new analyzer");

        group.MapPut("analyzers/{analyzerId}", async (IAnalyzerService analyzerService, Guid analyzerId, UpdateAnalyzerRequest request) =>
        {
            var result = await analyzerService.Update(request, analyzerId);
            return result.MapToResponse(analyzer => Results.Ok(analyzer));
        })
        .Produces<AnalyzerResponse>()
        .WithName("UpdateAnalyzer")
        .WithSummary("Update analyzer by id");

        group.MapGet("analyzers/{analyzerId:guid}/script", async (IAnalyzerService analyzerService, Guid analyzerId) =>
        {
            var result = await analyzerService.DownloadScript(analyzerId);
            return result.MapToResponse(file => Results.File(file.Stream, file.Metadata.ContentType, file.Metadata.FileName));
        })
        .Produces<FileStreamHttpResult>()
        .WithName("DownloadAnalyzerScript")
        .WithSummary("Download analyzer script");

        group.MapPost("analyzers/{analyzerId:guid}/script", async (IAnalyzerService analyzerService, IFormFile script, Guid analyzerId) =>
        {
            var result = await analyzerService.UploadScript(script, analyzerId);
            return result.MapToResponse(() => Results.Ok());
        })
        .Accepts<IFormFile>("multipart/form-data")
        .DisableAntiforgery()
        .WithName("UploadAnalyzerScript")
        .WithSummary("Upload analyzer script");

        group.MapGet("analyzers/{analyzerId:guid}/logs", async (IAnalyzerService analyzerService, Guid analyzerId) =>
        {
            var result = await analyzerService.GetLogsById(analyzerId);
            return result.MapToResponse(logs => Results.Ok(logs));
        })
        .Produces<IEnumerable<AnalyzerLogResponse>>()
        .WithName("GetAnalyzerLogs")
        .WithSummary("Get analyzer logs by analyzer id");

        group.MapDelete("analyzers/{analyzerId:guid}", async (IAnalyzerService analyzerService, Guid analyzerId) =>
        {
            var result = await analyzerService.DeleteById(analyzerId);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteAnalyzer")
        .WithSummary("Delete analyzer by id");
    }
}
