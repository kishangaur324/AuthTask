using AuthTask.Application.Enums;

namespace AuthTask.Application.DTOs
{
    /// <summary>
    /// Represents an application operation result with status, data, and error details.
    /// </summary>
    /// <typeparam name="T">Result payload type.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Gets the operation status.
        /// </summary>
        public ResultStatus Status { get; private set; }

        /// <summary>
        /// Gets the successful payload when available.
        /// </summary>
        public T? Data { get; private set; }

        /// <summary>
        /// Gets the error message when the operation fails.
        /// </summary>
        public string? Error { get; private set; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="data">Successful payload.</param>
        /// <returns>Success result.</returns>
        public static Result<T> Success(T data) =>
            new() { Status = ResultStatus.Success, Data = data };

        /// <summary>
        /// Creates a conflict result.
        /// </summary>
        /// <param name="error">Conflict message.</param>
        /// <returns>Conflict result.</returns>
        public static Result<T> Conflict(string error) =>
            new() { Status = ResultStatus.Conflict, Error = error };

        /// <summary>
        /// Creates a no-content result.
        /// </summary>
        /// <returns>No-content result.</returns>
        public static Result<T> NoContent() => new() { Status = ResultStatus.NoContent };

        /// <summary>
        /// Creates a not-found result.
        /// </summary>
        /// <param name="error">Not-found message.</param>
        /// <returns>Not-found result.</returns>
        public static Result<T> NotFound(string error) =>
            new() { Status = ResultStatus.NotFound, Error = error };

        /// <summary>
        /// Creates an unauthorized result.
        /// </summary>
        /// <param name="error">Unauthorized message.</param>
        /// <returns>Unauthorized result.</returns>
        public static Result<T> Unauthorized(string error) =>
            new() { Status = ResultStatus.Unauthorized, Error = error };

        /// <summary>
        /// Creates a failure result.
        /// </summary>
        /// <param name="error">Failure message.</param>
        /// <returns>Failure result.</returns>
        public static Result<T> Failure(string error) =>
            new() { Status = ResultStatus.Failure, Error = error };
    }
}
