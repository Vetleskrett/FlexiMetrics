namespace Api.Validation;

public enum ResultState
{
    Success,
    NoContent,
    NotFound,
    ValidationError,
    PermissionDenied
}

public sealed class Result
{
    private readonly ValidationResponse? _errors;
    private readonly ResultState _state;

    public Result(ValidationResponse errors)
    {
        _errors = errors;
        _state = ResultState.ValidationError;
    }

    private Result(ResultState state)
    {
        _state = state;
    }

    public static Result Success() => new(ResultState.Success);
    public static implicit operator Result(ValidationResponse errors) => new(errors);
    public static Result NoContent() => new(ResultState.NoContent);
    public static Result NotFound() => new(ResultState.NotFound);
    public static Result PermissionDenied() => new(ResultState.PermissionDenied);

    public IResult MapToResponse(Func<IResult> func)
    {
        return _state switch
        {
            ResultState.Success => func(),
            ResultState.NoContent => Results.NoContent(),
            ResultState.NotFound => Results.NotFound(),
            ResultState.ValidationError => Results.BadRequest(_errors!),
            ResultState.PermissionDenied => Results.Forbid(),
            _ => Results.InternalServerError("Unexpected ResultState"),
        };
    }
}

public sealed class Result<TValue> where TValue : notnull
{
    private readonly TValue? _value;
    private readonly ValidationResponse? _errors;
    private readonly ResultState _state;

    public Result(TValue value)
    {
        _value = value;
        _state = ResultState.Success;
    }

    public Result(ValidationResponse errors)
    {
        _errors = errors;
        _state = ResultState.ValidationError;
    }

    private Result(ResultState state)
    {
        _state = state;
    }

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(ValidationResponse errors) => new(errors);
    public static Result<TValue> NoContent() => new(ResultState.NoContent);
    public static Result<TValue> NotFound() => new(ResultState.NotFound);
    public static Result<TValue> PermissionDenied() => new(ResultState.PermissionDenied);

    public IResult MapToResponse(Func<TValue, IResult> func)
    {
        return _state switch
        {
            ResultState.Success => func(_value!),
            ResultState.NoContent => Results.NoContent(),
            ResultState.NotFound => Results.NotFound(),
            ResultState.ValidationError => Results.BadRequest(_errors!),
            ResultState.PermissionDenied => Results.Forbid(),
            _ => Results.InternalServerError("Unexpected ResultState"),
        };
    }
}
