namespace Api.Validation;

public class ValidationError
{
    public string? PropertyName { get; init; }
    public string Message { get; init; }

    public ValidationError(string message)
    {
        Message = message;
    }
}
