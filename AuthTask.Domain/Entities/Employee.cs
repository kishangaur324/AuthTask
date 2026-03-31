namespace AuthTask.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }

        public string? EmployeeCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }

        public Guid? DepartmentId { get; set; }
        public Guid? ManagerId { get; set; }

        public DateTime DateOfJoining { get; set; }
        public DateTime? DateOfLeaving { get; set; }

        public bool IsActive { get; set; }
        public required string UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
