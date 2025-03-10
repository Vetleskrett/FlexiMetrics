using Api.Analyses;
using Api.Analyzers.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Analyzers;

public static class AnalyzerEndpoints
{
    public static void MapAnalyzerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Analyzers").RequireAuthorization();

        group.MapGet("analyzers", async (IAnalyzerService analyzerService) =>
        {
            var result = await analyzerService.GetAll();
            return result.MapToResponse(analyzers => Results.Ok(analyzers));
        })
        .Produces<IEnumerable<AnalyzerResponse>>()
        .WithName("GetAllAnalyzers")
        .WithSummary("Get all analyzers");

        group.MapGet("analyzers/{id:guid}", async (IAnalyzerService analyzerService, Guid id) =>
        {
            var result = await analyzerService.GetById(id);
            return result.MapToResponse(analyzer => Results.Ok(analyzer));
        })
        .Produces<AnalyzerResponse>()
        .WithName("GetAnalyzer")
        .WithSummary("Get analyzer by id");

        group.MapPost("analyzers/{id:guid}/action", async (IAnalyzerService analyzerService, Guid id, AnalyzerActionRequest request) =>
        {
            var result = await analyzerService.StartAction(request, id);
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
                new { id = analyzer.Id },
                analyzer
            ));
        })
        .Produces<AnalyzerResponse>()
        .WithName("CreateAnalyzer")
        .WithSummary("Create new analyzer");

        group.MapPut("analyzers/{id}", async (IAnalyzerService analyzerService, Guid id, UpdateAnalyzerRequest request) =>
        {
            var result = await analyzerService.Update(request, id);
            return result.MapToResponse(analyzer => Results.Ok(analyzer));
        })
        .Produces<AnalyzerResponse>()
        .WithName("UpdateAnalyzer")
        .WithSummary("Update analyzer by id");

        group.MapGet("analyzers/{id:guid}/script", async (IAnalyzerService analyzerService, Guid id) =>
        {
            var result = await analyzerService.DownloadScript(id);
            return result.MapToResponse(file => Results.File(file.Stream, file.Metadata.ContentType, file.Metadata.FileName));
        })
        .Produces<FileStreamHttpResult>()
        .WithName("DownloadAnalyzerScript")
        .WithSummary("Download analyzer script");

        group.MapPost("analyzers/{id:guid}/script", async (IAnalyzerService analyzerService, IFormFile script, Guid id) =>
        {
            var result = await analyzerService.UploadScript(script, id);
            return result.MapToResponse(() => Results.Ok());
        })
        .Accepts<IFormFile>("multipart/form-data")
        .DisableAntiforgery()
        .WithName("UploadAnalyzerScript")
        .WithSummary("Upload analyzer script");

        group.MapGet("analyzers/{id:guid}/status", async (HttpContext context, IAnalyzerService analyzerService, Guid id) =>
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            context.Response.Headers.Append("Content-Type", "text/event-stream");
            await foreach (var statusUpdate in analyzerService.GetStatusEventsById(id, context.RequestAborted))
            {
                var json = JsonSerializer.Serialize(statusUpdate, jsonOptions);
                await context.Response.WriteAsync($"data: {json}\n\n");
                await context.Response.Body.FlushAsync();
            }
        })
        .Produces(StatusCodes.Status200OK, contentType: "text/event-stream")
        .WithName("GetAnalyzerStatus")
        .WithSummary("Get analyzer status by id");

        group.MapDelete("analyzers/{id:guid}", async (IAnalyzerService analyzerService, Guid id) =>
        {
            var result = await analyzerService.DeleteById(id);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteAnalyzer")
        .WithSummary("Delete analyzer by id");
    }
}
