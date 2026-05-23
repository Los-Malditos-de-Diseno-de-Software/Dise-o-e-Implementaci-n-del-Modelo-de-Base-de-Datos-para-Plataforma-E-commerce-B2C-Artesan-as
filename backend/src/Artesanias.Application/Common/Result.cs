namespace Artesanias.Application.Common;

public class Result<T>
{
    public bool Success { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public T? Data { get; private set; }
    public IEnumerable<string> Errors { get; private set; } = [];

    private Result() { }

    public static Result<T> Ok(T data, string message = "OK") =>
        new() { Success = true, Message = message, Data = data };

    public static Result<T> Fail(string message, IEnumerable<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? [] };

    public static Result<T> Fail(IEnumerable<string> errors) =>
        new() { Success = false, Message = "Validation failed", Errors = errors };
}

public class Result
{
    public bool Success { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public IEnumerable<string> Errors { get; private set; } = [];

    private Result() { }

    public static Result Ok(string message = "OK") =>
        new() { Success = true, Message = message };

    public static Result Fail(string message, IEnumerable<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? [] };

    public static Result Fail(IEnumerable<string> errors) =>
        new() { Success = false, Message = "Validation failed", Errors = errors };
}
