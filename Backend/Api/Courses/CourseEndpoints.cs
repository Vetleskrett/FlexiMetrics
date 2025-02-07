using Api.Courses.Contracts;

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

        group.MapGet("teachers/{teacherId:guid}/courses", async (ICourseService courseService, Guid teacherId) =>
        {
            var courses = await courseService.GetAllByTeacher(teacherId);
            return courses is not null ? Results.Ok(courses) : Results.NotFound();
        })
        .Produces<IEnumerable<CourseResponse>>()
        .WithName("GetAllCoursesByTeacher")
        .WithSummary("Get all courses by teacher id");

        group.MapGet("students/{studentId:guid}/courses", async (ICourseService courseService, Guid studentId) =>
        {
            var courses = await courseService.GetAllByStudent(studentId);
            return courses is not null ? Results.Ok(courses) : Results.NotFound();
        })
        .Produces<IEnumerable<CourseResponse>>()
        .WithName("GetAllCoursesByStudent")
        .WithSummary("Get all courses by student id");

        group.MapGet("/courses/{id:guid}", async (ICourseService courseService, Guid id) =>
        {
            var course = await courseService.GetById(id);
            return course is not null ? Results.Ok(course) : Results.NotFound();
        })
        .Produces<CourseResponse>()
        .WithName("GetCourse")
        .WithSummary("Get course by id");

        group.MapPost("courses", async (ICourseService courseService, CreateCourseRequest request) =>
        {
            var result = await courseService.Create(request);

            return result.Match
            (
                course => course is not null ? Results.CreatedAtRoute
                (
                    "GetCourse",
                    new { id = course.Id },
                    course
                ) : Results.NotFound(),
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
