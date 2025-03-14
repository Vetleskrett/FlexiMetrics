﻿using Api.Analyses.Contracts;

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

        group.MapGet("students/{studentId:guid}/assignments/{assignmentId:guid}/analyses", async (IAnalysisService analysisService, Guid studentId, Guid assignmentId) =>
        {
            var result = await analysisService.GetStudentAssignmentAnalyses(studentId, assignmentId);
            return result.MapToResponse(analysis => Results.Ok(analysis));
        })
        .Produces<IEnumerable<StudentAnalysisResponse>>()
        .WithName("GetAnalysesByStudentAssignment")
        .WithSummary("Get analyses by student id and assignment id");

        group.MapGet("teams/{teamId:guid}/assignments/{assignmentId:guid}/analyses", async (IAnalysisService analysisService, Guid teamId, Guid assignmentId) =>
        {
            var result = await analysisService.GetTeamAssignmentAnalyses(teamId, assignmentId);
            return result.MapToResponse(analysis => Results.Ok(analysis));
        })
        .Produces<IEnumerable<StudentAnalysisResponse>>()
        .WithName("GetAnalysesByTeamAssignment")
        .WithSummary("Get analyses by team id and assignment id");

        group.MapDelete("analyses/{id:guid}", async (IAnalysisService analysisService, Guid id) =>
        {
            var result = await analysisService.DeleteById(id);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteAnalysis")
        .WithSummary("Delete analysis by id");
    }
}
