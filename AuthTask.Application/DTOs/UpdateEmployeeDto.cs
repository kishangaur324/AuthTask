namespace AuthTask.Application.DTOs
{
    public class UpdateEmployeeDto
    {
        public Guid Id { get; set; }
        public required string EmployeeCode { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? ManagerId { get; set; }

        public DateTime DateOfJoining { get; set; }
        public DateTime? DateOfLeaving { get; set; }

        public bool IsActive { get; set; }
    }
}
