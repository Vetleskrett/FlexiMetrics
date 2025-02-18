using Api.Students.Contracts;

namespace Api.Students;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Students");

        group.MapGet("students/{studentId:guid}", async (IStudentService studentService, Guid studentId) =>
        {
            var result = await studentService.GetById(studentId);
            return result.MapToResponse(student => Results.Ok(student));
        })
        .Produces<StudentResponse>()
        .WithName("GetStudent")
        .WithSummary("Get student by id");

        group.MapGet("courses/{courseId:guid}/students", async (IStudentService studentService, Guid courseId) =>
        {
            var result = await studentService.GetAllByCourse(courseId);
            return result.MapToResponse(students => Results.Ok(students));
        })
        .Produces<IEnumerable<StudentResponse>>()
        .WithName("GetAllStudentsByCourse")
        .WithSummary("Get all students by course id");

        group.MapPost("courses/{courseId:guid}/students", async (IStudentService studentService, Guid courseId, AddStudentsToCourseRequest request) =>
        {
            var result = await studentService.AddToCourse(courseId, request);
            return result.MapToResponse(students => Results.Ok(students));
        })
        .Produces<IEnumerable<StudentResponse>>()
        .WithName("AddStudentsToCourse")
        .WithSummary("Add students to course");

        group.MapDelete("courses/{courseId:guid}/students/{studentId:guid}", async (IStudentService studentService, Guid courseId, Guid studentId) =>
        {
            var result = await studentService.RemoveFromCourse(courseId, studentId);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("RemoveStudentFromCourse")
        .WithSummary("Remove student from course");
    }
}