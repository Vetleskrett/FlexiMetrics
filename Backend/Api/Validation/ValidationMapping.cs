using FluentValidation.Results;

namespace Api.Validation;

public static class ValidationMapping
{
    public static ValidationResponse MapToResponse(this IEnumerable<ValidationFailure> failures)
    {
        return new ValidationResponse
        {
            Errors = failures.Select(x => new ValidationError(x.ErrorMessage)
            {
                PropertyName = x.PropertyName
            })
        };
    }

    public static ValidationResponse MapToResponse(this ValidationError error)
    {
        return new ValidationResponse
        {
            Errors = [error]
        };
    }
}
