namespace Api.Courses.Contracts;

public class CourseResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Code { get; init; }
}
