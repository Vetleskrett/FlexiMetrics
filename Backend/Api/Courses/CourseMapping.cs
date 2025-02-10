using Api.Courses.Contracts;
using Database.Models;

namespace Api.Courses;

public static class CourseMapping
{
    public static Course MapToCourse(this CreateCourseRequest request)
    {
        return new Course
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            Year = request.Year,
            Semester = request.Semester,
        };
    }

    public static Course MapToCourse(this UpdateCourseRequest request, Guid id)
    {
        return new Course
        {
            Id = id,
            Code = request.Code,
            Name = request.Name,
            Year = request.Year,
            Semester = request.Semester,
        };
    }

    public static CourseResponse MapToResponse(this Course course)
    {
        return new CourseResponse
        {
            Id = course.Id,
            Code = course.Code,
            Name = course.Name,
            Year = course.Year,
            Semester = course.Semester,
        };
    }

    public static List<CourseResponse> MapToResponse(this IEnumerable<Course> courses)
    {
        return courses.Select(course => course.MapToResponse()).ToList();
    }
}
