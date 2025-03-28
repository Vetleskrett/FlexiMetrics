using Api.Progress.Contracts;

namespace Api.Progress;

public static class ProgressEndpoints
{
    public static void MapProgressEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Progress").RequireAuthorization();

        group.MapGet("courses/{courseId:guid}/progress/students", async (IProgressService progressService, Guid courseId) =>
        {
            var result = await progressService.GetCourseStudentsProgress(courseId);
            return result.MapToResponse(progress => Results.Ok(progress));
        })
        .Produces<IEnumerable<ProgressResponse>>()
        .WithName("GetStudentsProgressByCourse")
        .RequireAuthorization("TeacherInCourse")
        .WithSummary("Get students progress by course id");

        group.MapGet("courses/{courseId:guid}/progress/teams", async (IProgressService progressService, Guid courseId) =>
        {
            var result = await progressService.GetCourseTeamsProgress(courseId);
            return result.MapToResponse(progress => Results.Ok(progress));
        })
        .Produces<IEnumerable<ProgressResponse>>()
        .WithName("GetTeamsProgressByCourse")
        .RequireAuthorization("TeacherInCourse")
        .WithSummary("Get teams progress by course id");
    }
}
