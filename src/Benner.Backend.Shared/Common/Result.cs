namespace Benner.Backend.Shared.Common;

public class Result
{
    private Result(bool isSuccess, string error, List<string> errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? [];
    }

    public bool IsSuccess { get; }
    public string Error { get; }
    public List<string> Errors { get; }

    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    public static Result Failure(string error)
    {
        return new Result(false, error);
    }

    public static Result Failure(List<string> errors)
    {
        return new Result(false, string.Empty, errors);
    }
}