using Api.Students.Contracts;

namespace Api.Students;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Students");

        group.MapGet("courses/{courseId:guid}/students", async (IStudentService studentService, Guid courseId) =>
        {
            var courses = await studentService.GetAllByCourse(courseId);
            return courses is not null ? Results.Ok(courses) : Results.NotFound();
        })
        .Produces<IEnumerable<StudentResponse>>()
        .WithName("GetAllStudentsByCourse")
        .WithSummary("Get all students by course id");

        group.MapPost("courses/{courseId:guid}/students", async (IStudentService studentService, Guid courseId, AddStudentsToCourseRequest request) =>
        {
            var added = await studentService.AddToCourse(courseId, request);
            return added ? Results.Ok() : Results.NotFound();
        })
        .WithName("AddStudentsToCourse")
        .WithSummary("Add students to course");

        group.MapDelete("courses/{courseId:guid}/students/{studentId:guid}", async (IStudentService studentService, Guid courseId, Guid studentId) =>
        {
            var result = await studentService.RemoveFromCourse(courseId, studentId);
            return result.Match
            (
                deleted => deleted ? Results.Ok() : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .WithName("RemoveStudentFromCourse")
        .WithSummary("Remove student from course");
    }
}