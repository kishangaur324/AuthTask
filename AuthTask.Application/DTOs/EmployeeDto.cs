namespace AuthTask.Application.DTOs
{
    /// <summary>
    /// Employee data returned by read endpoints.
    /// </summary>
    public class EmployeeDto
    {
        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the employee code.
        /// </summary>
        public required string EmployeeCode { get; set; }

        /// <summary>
        /// Gets or sets the employee first name.
        /// </summary>
        public required string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the employee last name.
        /// </summary>
        public required string LastName { get; set; }

        /// <summary>
        /// Gets or sets the employee email.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the employee phone number.
        /// </summary>
        public required string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the joining date.
        /// </summary>
        public DateTime DateOfJoining { get; set; }

        /// <summary>
        /// Gets or sets the leaving date, when available.
        /// </summary>
        public DateTime? DateOfLeaving { get; set; }

        /// <summary>
        /// Gets or sets whether the employee is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
