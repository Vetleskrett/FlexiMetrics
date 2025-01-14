using Database.Models;

namespace Api.Courses.Contracts;

public class CourseResponse
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required int Year { get; init; }
    public required Semester Semester { get; init; }
}
