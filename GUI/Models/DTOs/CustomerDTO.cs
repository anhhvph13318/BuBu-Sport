namespace GUI.Models.DTOs
{
    public class CustomerDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; } 
        public string? Address { get; set; }
        public string? Status { get; set; }
        public int? Point { get; set; }
    }
}
