namespace Api.Students.Contracts;

public class AddStudentsToCourseRequest
{
    public required List<string> Emails { get; init; }
}
