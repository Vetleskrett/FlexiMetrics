namespace Api.Courses.Contracts;

public class UpdateCourseRequest
{
    public required string Title { get; init; }
    public required string Code { get; init; }
}
