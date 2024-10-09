using Api.Courses.Contracts;
using Api.Validation;

namespace Api.Courses;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("courses").WithTags("Courses");

        group.MapGet("/", async (ICourseService courseService) =>
        {
            var courses = await courseService.GetAll();
            var coursesResponse = courses.MapToResponse();
            return Results.Ok(coursesResponse);
        })
        .Produces<IEnumerable<CourseResponse>>()
        .WithName("GetAllCourses")
        .WithSummary("Get all courses");

        group.MapGet("/{id:guid}", async (ICourseService courseService, Guid id) =>
        {
            var result = await courseService.GetById(id);
            if (result is not null)
            {
                return Results.Ok(result.MapToResponse());
            }
            else
            {
                return Results.NotFound();
            }
        })
        .Produces<CourseResponse>()
        .WithName("GetCourse")
        .WithSummary("Get course by id");

        group.MapPost("/", async (ICourseService courseService, CreateCourseRequest request) =>
        {
            var course = request.MapToCourse();
            var result = await courseService.Create(course);

            return result.Match
            (
                course => Results.CreatedAtRoute
                    (
                        "GetCourse",
                        new { id = course.Id },
                        course.MapToResponse()
                    ),
                failed => Results.BadRequest(failed.MapToResponse())
            );
        })
        .Produces<CourseResponse>()
        .WithName("CreateCourse")
        .WithSummary("Create new course");

        group.MapPut("/{id:guid}", async (ICourseService courseService, Guid id, UpdateCourseRequest request) =>
        {
            var course = request.MapToCourse(id);
            var result = await courseService.Update(course);

            return result.Match
            (
                course =>
                {
                    if (course is not null)
                    {
                        return Results.Ok(course.MapToResponse());
                    }
                    else
                    {
                        return Results.NotFound();
                    }
                },
                failed => Results.BadRequest(failed.MapToResponse())
            );
        })
        .Produces<CourseResponse>()
        .WithName("UpdateCourse")
        .WithSummary("Update course by id");

        group.MapDelete("/{id:guid}", async (ICourseService courseService, Guid id) =>
        {
            var deleted = await courseService.DeleteById(id);
            return deleted ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteCourse")
        .WithSummary("Delete course by id");
    }
}
