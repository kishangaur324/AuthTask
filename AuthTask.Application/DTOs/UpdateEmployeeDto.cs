namespace AuthTask.Application.DTOs
{
    /// <summary>
    /// Payload used to update an employee record.
    /// </summary>
    public class UpdateEmployeeDto
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
        /// Gets or sets the department identifier.
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the manager identifier.
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Gets or sets the employee joining date.
        /// </summary>
        public DateTime DateOfJoining { get; set; }

        /// <summary>
        /// Gets or sets the employee leaving date.
        /// </summary>
        public DateTime? DateOfLeaving { get; set; }

        /// <summary>
        /// Gets or sets whether the employee is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
