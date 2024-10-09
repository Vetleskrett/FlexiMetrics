namespace Api.Courses.Contracts;

public class CreateCourseRequest
{
    public required string Title { get; init; }
    public required string Code { get; init; }
}
