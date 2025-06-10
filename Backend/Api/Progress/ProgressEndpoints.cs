using Api.Progress.Contracts;

namespace Api.Progress;

public static class ProgressEndpoints
{
    public static void MapProgressEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Progress");

        group.MapGet("courses/{courseId:guid}/progress/students", async (IProgressService progressService, Guid courseId) =>
        {
            var result = await progressService.GetCourseStudentsProgress(courseId);
            return result.MapToResponse(progress => Results.Ok(progress));
        })
        .Produces<IEnumerable<SlimProgressResponse>>()
        .WithName("GetStudentsProgressByCourse")
        .WithSummary("Get students progress by course id");

        group.MapGet("courses/{courseId:guid}/progress/teams", async (IProgressService progressService, Guid courseId) =>
        {
            var result = await progressService.GetCourseTeamsProgress(courseId);
            return result.MapToResponse(progress => Results.Ok(progress));
        })
        .Produces<IEnumerable<SlimProgressResponse>>()
        .WithName("GetTeamsProgressByCourse")
        .WithSummary("Get teams progress by course id");

        group.MapGet("courses/{courseId:guid}/progress/students/{studentId:guid}", async (IProgressService progressService, Guid courseId, Guid studentId) =>
        {
            var result = await progressService.GetCourseStudentProgress(courseId, studentId);
            return result.MapToResponse(progress => Results.Ok(progress));
        })
        .Produces<IEnumerable<AssignmentProgressResponse>>()
        .WithName("GetStudentProgressByCourse")
        .WithSummary("Get student progress by course id and student id");

        group.MapGet("courses/{courseId:guid}/progress/teams/{teamId:guid}", async (IProgressService progressService, Guid courseId, Guid teamId) =>
        {
            var result = await progressService.GetCourseTeamProgress(courseId, teamId);
            return result.MapToResponse(progress => Results.Ok(progress));
        })
        .Produces<IEnumerable<AssignmentProgressResponse>>()
        .WithName("GetTeamProgressByCourse")
        .WithSummary("Get team progress by course id and team id");
    }
}
