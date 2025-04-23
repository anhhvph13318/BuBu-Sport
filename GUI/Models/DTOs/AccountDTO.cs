namespace GUI.Models.DTOs
{
    public class AccountDTO
    {
        public Guid Id { get; set; }
        public string? AccountCode { get; set; }
        public string? FullName { get; set; } 
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public Guid? CustomerID { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
