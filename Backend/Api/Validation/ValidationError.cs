namespace Api.Validation;

public class ValidationError
{
    public required string PropertyName { get; init; }
    public required string Message { get; init; }
}
