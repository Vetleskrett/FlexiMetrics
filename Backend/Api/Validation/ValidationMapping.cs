using FluentValidation.Results;
using Movies.Api.Contracts.Responses;

namespace Api.Validation;

public static class ValidationMapping
{
    public static ValidationResponse MapToResponse(this IEnumerable<ValidationFailure> failures)
    {
        return new ValidationResponse
        {
            Errors = failures.Select(x => new ValidationError
            {
                PropertyName = x.PropertyName,
                Message = x.ErrorMessage
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
