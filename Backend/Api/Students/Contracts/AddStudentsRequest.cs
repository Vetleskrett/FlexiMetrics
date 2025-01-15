namespace Api.Students.Contracts;

public class AddStudentsRequest
{
    public required List<string> Emails { get; init; }
}
