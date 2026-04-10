namespace AuthTask.Application.DTOs
{
    /// <summary>
    /// Payload used to create an employee record.
    /// </summary>
    public class CreateEmployeeDto
    {
        /// <summary>
        /// Gets or sets the identity user identifier associated with the employee.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Gets or sets the employee code.
        /// </summary>
        public string? EmployeeCode { get; set; }

        /// <summary>
        /// Gets or sets the employee first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the employee last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the employee email.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the employee phone number.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the employee joining date.
        /// </summary>
        public DateTime DateOfJoining { get; set; }
    }
}
