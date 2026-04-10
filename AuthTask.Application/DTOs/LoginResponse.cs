namespace AuthTask.Application.DTOs
{
    /// <summary>
    /// Response payload containing issued authentication token.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the JWT access token.
        /// </summary>
        public required string AccessToken { get; set; }
    }
}
