namespace Job4Students.Shared;

public class Result
{
    public bool HasError { get; set; }
    public bool IsSuccess
    {
        get { return HasError == false; }
        set { HasError = value == false; }
    }

    public string Message { get; set; } = string.Empty;

    public static Result Success(string message = "Success")
    {
        return new Result
        {
            HasError = false,
            Message = message
        };
    }

    public static Result Failure(string message)
    {
        return new Result
        {
            HasError = true,
            Message = message
        };
    }
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data, string message = "Success")
    {
        return new Result<T>
        {
            HasError = false,
            Message = message,
            Data = data
        };
    }

    public new static Result<T> Failure(string message)
    {
        return new Result<T>
        {
            HasError = true,
            Message = message
        };
    }
}
