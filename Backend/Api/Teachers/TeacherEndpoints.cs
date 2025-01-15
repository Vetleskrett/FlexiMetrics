using Api.Teachers.Contracts;

namespace Api.Teachers;

public static class TeacherEndpoints
{
    public static void MapTeacherEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Teachers");

        group.MapGet("courses/{courseId:guid}/teachers", async (ITeacherService teacherService, Guid courseId) =>
        {
            var courses = await teacherService.GetAllByCourseId(courseId);
            return courses is not null ? Results.Ok(courses) : Results.NotFound();
        })
        .Produces<IEnumerable<TeacherResponse>>()
        .WithName("GetAllTeachersByCourseId")
        .WithSummary("Get all teachers by course id");

        group.MapPost("courses/{courseId:guid}/teachers", async (ITeacherService teacherService, Guid courseId, AddStudentRequest request) =>
        {
            var added = await teacherService.AddToCourse(courseId, request);
            return added ? Results.Ok() : Results.NotFound();
        })
        .WithName("AddTeacherToCourse")
        .WithSummary("Add teacher to course");

        group.MapDelete("courses/{courseId:guid}/teachers/{teacherId:guid}", async (ITeacherService teacherService, Guid courseId, Guid teacherId) =>
        {
            var result = await teacherService.RemoveFromCourse(courseId, teacherId);
            return result.Match
            (
                deleted => deleted ? Results.Ok() : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .WithName("RemoveTeacherFromCourse")
        .WithSummary("Remove teacher from course");
    }
}