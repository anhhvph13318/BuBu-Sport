namespace GUI.Models.DTOs
{
    public class AccountDTO
    {
        public Guid Id { get; set; }
        public string? AccountCode { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        //public bool? Status { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
