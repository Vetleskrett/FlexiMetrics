using Api.Teachers.Contracts;

namespace Api.Teachers;

public static class TeacherEndpoints
{
    public static void MapTeacherEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Teachers");

        group.MapGet("courses/{courseId:guid}/teachers", async (ITeacherService teacherService, Guid courseId) =>
        {
            var result = await teacherService.GetAllByCourse(courseId);
            return result.MapToResponse(teachers => Results.Ok(teachers));
        })
        .Produces<IEnumerable<TeacherResponse>>()
        .WithName("GetAllTeachersByCourse")
        .WithSummary("Get all teachers by course id");

        group.MapPost("courses/{courseId:guid}/teachers", async (ITeacherService teacherService, Guid courseId, AddTeacherRequest request) =>
        {
            var result = await teacherService.AddToCourse(courseId, request);
            return result.MapToResponse(teachers => Results.Ok(teachers));
        })
        .Produces<IEnumerable<TeacherResponse>>()
        .WithName("AddTeacherToCourse")
        .WithSummary("Add teacher to course");

        group.MapDelete("courses/{courseId:guid}/teachers/{teacherId:guid}", async (ITeacherService teacherService, Guid courseId, Guid teacherId) =>
        {
            var result = await teacherService.RemoveFromCourse(courseId, teacherId);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("RemoveTeacherFromCourse")
        .WithSummary("Remove teacher from course");
    }
}