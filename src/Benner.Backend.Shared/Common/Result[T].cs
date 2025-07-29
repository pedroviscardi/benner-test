namespace Benner.Backend.Shared.Common;

public class Result<T>
{
    private Result(bool isSuccess, T data, string error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        Errors = errors ?? [];
    }

    public bool IsSuccess { get; }
    public T Data { get; }
    public string Error { get; }
    public List<string> Errors { get; }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, string.Empty);
    }

    public static Result<T?> Failure(string error)
    {
        return new Result<T?>(false, default, error);
    }

    public static Result<T?> Failure(List<string> errors)
    {
        return new Result<T?>(false, default, string.Empty, errors);
    }
}