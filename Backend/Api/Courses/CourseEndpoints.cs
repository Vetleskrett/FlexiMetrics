using Api.Courses;
using Api.Courses.Contracts;
using Api.Validation;

namespace Api.Courses;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Courses");

        group.MapGet("courses", async (ICourseService courseService) =>
        {
            var courses = await courseService.GetAll();
            return Results.Ok(courses);
        })
        .Produces<IEnumerable<CourseResponse>>()
        .WithName("GetAllCourses")
        .WithSummary("Get all courses");

        group.MapGet("teacher/{teacherId:guid}/courses", async (ICourseService courseService, Guid teacherId) =>
        {
            var courses = await courseService.GetAllByTeacherId(teacherId);
            return Results.Ok(courses);
        })
        .Produces<IEnumerable<CourseResponse>>()
        .WithName("GetAllCoursesByTeacherId")
        .WithSummary("Get all courses by teacher id");

        group.MapGet("student/{studentId:guid}/courses", async (ICourseService courseService, Guid studentId) =>
        {
            var courses = await courseService.GetAllByStudentId(studentId);
            return Results.Ok(courses);
        })
        .Produces<IEnumerable<CourseResponse>>()
        .WithName("GetAllCoursesByStudentId")
        .WithSummary("Get all courses by student id");

        group.MapGet("courses/{id:guid}", async (ICourseService courseService, Guid id) =>
        {
            var course = await courseService.GetById(id);
            if (course is not null)
            {
                return Results.Ok(course);
            }
            else
            {
                return Results.NotFound();
            }
        })
        .Produces<CourseFullResponse>()
        .WithName("GetCourse")
        .WithSummary("Get course by id");

        group.MapPost("courses", async (ICourseService courseService, CreateCourseRequest request) =>
        {
            var result = await courseService.Create(request);

            return result.Match
            (
                course => Results.CreatedAtRoute
                (
                    "GetCourse",
                    new { id = course.Id },
                    course
                ),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<CourseResponse>()
        .WithName("CreateCourse")
        .WithSummary("Create new course");

        group.MapPut("courses/{id:guid}", async (ICourseService courseService, Guid id, UpdateCourseRequest request) =>
        {
            var result = await courseService.Update(request, id);

            return result.Match
            (
                course => course is not null ? Results.Ok(course) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<CourseResponse>()
        .WithName("UpdateCourse")
        .WithSummary("Update course by id");

        group.MapDelete("courses/{id:guid}", async (ICourseService courseService, Guid id) =>
        {
            var deleted = await courseService.DeleteById(id);
            return deleted ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteCourse")
        .WithSummary("Delete course by id");
    }
}
