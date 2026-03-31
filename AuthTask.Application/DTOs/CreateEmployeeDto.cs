namespace AuthTask.Application.DTOs
{
    public class CreateEmployeeDto
    {
        public required string UserId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }

        public DateTime DateOfJoining { get; set; }
    }
}
