namespace Api.Validation;

public class ValidationResponse
{
    public required IEnumerable<ValidationError> Errors { get; init; }
}
