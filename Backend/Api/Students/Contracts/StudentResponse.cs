namespace Api.Students.Contracts;

public class StudentResponse
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }
}