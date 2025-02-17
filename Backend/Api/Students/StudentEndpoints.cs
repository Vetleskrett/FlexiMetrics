using Api.Students.Contracts;

namespace Api.Students;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Students");

        group.MapGet("students/{studentId:guid}", async (IStudentService studentService, Guid studentId) =>
        {
            var student = await studentService.GetById(studentId);
            return student is not null ? Results.Ok(student) : Results.NotFound();
        })
        .Produces<StudentResponse>()
        .WithName("GetStudent")
        .WithSummary("Get student by id");

        group.MapGet("courses/{courseId:guid}/students", async (IStudentService studentService, Guid courseId) =>
        {
            var students = await studentService.GetAllByCourse(courseId);
            return students is not null ? Results.Ok(students) : Results.NotFound();
        })
        .Produces<IEnumerable<StudentResponse>>()
        .WithName("GetAllStudentsByCourse")
        .WithSummary("Get all students by course id");

        group.MapPost("courses/{courseId:guid}/students", async (IStudentService studentService, Guid courseId, AddStudentsToCourseRequest request) =>
        {
            var students = await studentService.AddToCourse(courseId, request);
            return students is not null ? Results.Ok(students) : Results.NotFound();
        })
        .Produces<IEnumerable<StudentResponse>>()
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