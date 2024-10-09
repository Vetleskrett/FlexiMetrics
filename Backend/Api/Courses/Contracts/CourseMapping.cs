using Database.Models;

namespace Api.Courses.Contracts;

public static class CourseMapping
{
    public static Course MapToCourse(this CreateCourseRequest request)
    {
        return new Course
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Code = request.Code,
        };
    }

    public static Course MapToCourse(this UpdateCourseRequest request, Guid id)
    {
        return new Course
        {
            Id = id,
            Title = request.Title,
            Code = request.Code,
        };
    }

    public static CourseResponse MapToResponse(this Course course)
    {
        return new CourseResponse
        {
            Id = course.Id,
            Title = course.Title,
            Code = course.Code,
        };
    }

    public static IEnumerable<CourseResponse> MapToResponse(this IEnumerable<Course> courses)
    {
        return courses.Select(course => course.MapToResponse());
    }
}
