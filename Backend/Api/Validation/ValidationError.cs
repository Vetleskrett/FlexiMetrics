namespace Movies.Api.Contracts.Responses;

public class ValidationError
{
    public required string PropertyName { get; init; }
    public required string Message { get; init; }
}
