namespace SurveryBasket.Api.Abstractions;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException();
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailed => !IsSuccess;
    public Error Error { get; } = default!;
    public static Result Success() => new Result(true, Error.None);
    public static Result Failure(Error error) => new Result(false, error);
    public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(default,false, error);
    public static Result<TValue> Success<TValue>(TValue value) => new Result<TValue>(value, true, Error.None);
   

}

public record Error(string Code, string Description,int? statusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty,null);
}

public class Result<TValue>(TValue? value , bool isSuccess , Error error ) : Result(isSuccess,error)
{

    private readonly TValue? _value  = value;

    public TValue Value => IsSuccess? _value! : throw new InvalidOperationException();  
}
