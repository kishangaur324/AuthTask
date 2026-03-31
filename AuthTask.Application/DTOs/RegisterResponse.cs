namespace AuthTask.Application.DTOs
{
    public class RegisterResponse
    {
        public required Guid EmployeeId { get; set; }
        public required string Email { get; set; }
    }
}
