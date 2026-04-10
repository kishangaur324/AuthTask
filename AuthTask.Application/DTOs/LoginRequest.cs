namespace AuthTask.Application.DTOs
{
    /// <summary>
    /// Request payload used for user authentication.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        public required string Password { get; set; }
    }
}
