using Api.Analyses.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

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

        group.MapGet("analyses/{analysisId:guid}", async (IAnalysisService analysisService, Guid analysisId) =>
        {
            var result = await analysisService.GetById(analysisId);
            return result.MapToResponse(analysis => Results.Ok(analysis));
        })
        .Produces<AnalysisResponse>()
        .WithName("GetAnalysis")
        .RequireAuthorization("TeacherForAnalysis")
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
        .RequireAuthorization("TeacherForAssignmentOrStudent")
        .WithSummary("Get analyses by student id and assignment id");

        group.MapGet("teams/{teamId:guid}/assignments/{assignmentId:guid}/analyses", async (IAnalysisService analysisService, Guid teamId, Guid assignmentId) =>
        {
            var result = await analysisService.GetTeamAssignmentAnalyses(teamId, assignmentId);
            return result.MapToResponse(analysis => Results.Ok(analysis));
        })
        .Produces<IEnumerable<StudentAnalysisResponse>>()
        .WithName("GetAnalysesByTeamAssignment")
        .RequireAuthorization("TeacherForAssignmentOrTeam")
        .WithSummary("Get analyses by team id and assignment id");

        group.MapGet("analysis-fields/{analysisFieldId:guid}", async (IAnalysisService analysisService, Guid analysisFieldId) =>
        {
            var result = await analysisService.DownloadFile(analysisFieldId);
            return result.MapToResponse(file => Results.File(file.Stream, file.Metadata.ContentType, file.Metadata.FileName));
        })
        .Produces<FileStreamHttpResult>()
        .WithName("DownloadAnalysisFile")
        .WithSummary("Download analysis file");

        group.MapDelete("analyses/{analysisId:guid}", async (IAnalysisService analysisService, Guid analysisId) =>
        {
            var result = await analysisService.DeleteById(analysisId);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteAnalysis")
        .RequireAuthorization("TeacherForAnalysis")
        .WithSummary("Delete analysis by id");
    }
}
