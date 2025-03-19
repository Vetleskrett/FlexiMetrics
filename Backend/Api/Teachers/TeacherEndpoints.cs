using Api.Teachers.Contracts;

namespace Api.Teachers;

public static class TeacherEndpoints
{
    public static void MapTeacherEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Teachers").RequireAuthorization();

        group.MapGet("courses/{courseId:guid}/teachers", async (ITeacherService teacherService, Guid courseId) =>
        {
            var result = await teacherService.GetAllByCourse(courseId);
            return result.MapToResponse(teachers => Results.Ok(teachers));
        })
        .Produces<IEnumerable<TeacherResponse>>()
        .WithName("GetAllTeachersByCourse")
        .RequireAuthorization("Course")
        .WithSummary("Get all teachers by course id");

        group.MapPost("courses/{courseId:guid}/teachers", async (ITeacherService teacherService, Guid courseId, AddTeacherRequest request) =>
        {
            var result = await teacherService.AddToCourse(courseId, request);
            return result.MapToResponse(teachers => Results.Ok(teachers));
        })
        .Produces<IEnumerable<TeacherResponse>>()
        .WithName("AddTeacherToCourse")
        .RequireAuthorization("TeacherInCourse")
        .WithSummary("Add teacher to course");

        group.MapDelete("courses/{courseId:guid}/teachers/{teacherId:guid}", async (ITeacherService teacherService, Guid courseId, Guid teacherId) =>
        {
            var result = await teacherService.RemoveFromCourse(courseId, teacherId);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("RemoveTeacherFromCourse")
        .RequireAuthorization("TeacherInCourse")
        .WithSummary("Remove teacher from course");
    }
}