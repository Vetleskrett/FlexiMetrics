namespace Movies.Api.Contracts.Responses;

public class ValidationResponse
{
    public required IEnumerable<ValidationError> Errors { get; init; }
}
