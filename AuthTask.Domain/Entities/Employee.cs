namespace AuthTask.Domain.Entities
{
    /// <summary>
    /// Employee aggregate linked to an identity user.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the employee code.
        /// </summary>
        public string? EmployeeCode { get; set; }

        /// <summary>
        /// Gets or sets first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets email address.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets phone number.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets department identifier.
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets manager identifier.
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Gets or sets joining date.
        /// </summary>
        public DateTime DateOfJoining { get; set; }

        /// <summary>
        /// Gets or sets leaving date.
        /// </summary>
        public DateTime? DateOfLeaving { get; set; }

        /// <summary>
        /// Gets or sets whether employee is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets linked identity user identifier.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Gets or sets navigation property to linked identity user.
        /// </summary>
        public virtual User? User { get; set; }
    }
}
