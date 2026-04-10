namespace AuthTask.Application.DTOs
{
    /// <summary>
    /// Response payload returned after successful user registration.
    /// </summary>
    public class RegisterResponse
    {
        /// <summary>
        /// Gets or sets the employee identifier linked to the created user.
        /// </summary>
        public required Guid EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the registered email.
        /// </summary>
        public required string Email { get; set; }
    }
}
