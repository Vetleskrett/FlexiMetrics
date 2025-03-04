using Api.Courses.Contracts;

namespace Api.Courses;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Courses").RequireAuthorization();

        group.MapGet("courses", async (ICourseService courseService) =>
        {
            var result = await courseService.GetAll();
            return result.MapToResponse(courses =>Results.Ok(courses));
        })
        .Produces<IEnumerable<CourseResponse>>()
        .WithName("GetAllCourses")
        .WithSummary("Get all courses");

        group.MapGet("teachers/{teacherId:guid}/courses", async (ICourseService courseService, Guid teacherId) =>
        {
            var result = await courseService.GetAllByTeacher(teacherId);
            return result.MapToResponse(courses => Results.Ok(courses));
        })
        .Produces<IEnumerable<CourseResponse>>()
        .WithName("GetAllCoursesByTeacher")
        .WithSummary("Get all courses by teacher id");

        group.MapGet("students/{studentId:guid}/courses", async (ICourseService courseService, Guid studentId) =>
        {
            var result = await courseService.GetAllByStudent(studentId);
            return result.MapToResponse(courses => Results.Ok(courses));
        })
        .Produces<IEnumerable<CourseResponse>>()
        .WithName("GetAllCoursesByStudent")
        .WithSummary("Get all courses by student id");

        group.MapGet("/courses/{id:guid}", async (ICourseService courseService, Guid id) =>
        {
            var result = await courseService.GetById(id);
            return result.MapToResponse(course => Results.Ok(course));
        })
        .Produces<CourseResponse>()
        .WithName("GetCourse")
        .WithSummary("Get course by id");

        group.MapPost("courses", async (ICourseService courseService, CreateCourseRequest request) =>
        {
            var result = await courseService.Create(request);
            return result.MapToResponse(course => Results.CreatedAtRoute
            (
                "GetCourse",
                new { id = course.Id },
                course
            ));
        })
        .Produces<CourseResponse>()
        .WithName("CreateCourse")
        .WithSummary("Create new course");

        group.MapPut("courses/{id:guid}", async (ICourseService courseService, Guid id, UpdateCourseRequest request) =>
        {
            var result = await courseService.Update(request, id);
            return result.MapToResponse(course => Results.Ok(course));
        })
        .Produces<CourseResponse>()
        .WithName("UpdateCourse")
        .WithSummary("Update course by id");

        group.MapDelete("courses/{id:guid}", async (ICourseService courseService, Guid id) =>
        {
            var result = await courseService.DeleteById(id);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteCourse")
        .WithSummary("Delete course by id");
    }
}
