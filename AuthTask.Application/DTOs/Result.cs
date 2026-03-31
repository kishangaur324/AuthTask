using AuthTask.Application.Enums;

namespace AuthTask.Application.DTOs
{
    public class Result<T>
    {
        public ResultStatus Status { get; private set; }
        public T? Data { get; private set; }
        public string? Error { get; private set; }

        public static Result<T> Success(T data) =>
            new() { Status = ResultStatus.Success, Data = data };

        public static Result<T> Conflict(string error) =>
            new() { Status = ResultStatus.Conflict, Error = error };

        public static Result<T> NoContent() => new() { Status = ResultStatus.NoContent };

        public static Result<T> NotFound(string error) =>
            new() { Status = ResultStatus.NotFound, Error = error };

        public static Result<T> Unauthorized(string error) =>
            new() { Status = ResultStatus.Unauthorized, Error = error };

        public static Result<T> Failure(string error) =>
            new() { Status = ResultStatus.Failure, Error = error };
    }
}
