namespace AuthTask.Application.DTOs
{
    /// <summary>
    /// Request payload used to register a new user.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// Gets or sets the role assigned at registration.
        /// </summary>
        public required string Role { get; set; }
    }
}
