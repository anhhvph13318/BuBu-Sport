namespace GUI.Models.DTOs.Customer_DTO
{
    public class EditCustomerRequest : CreateCustomerRequest
    {
        public Guid Id { get; set; }

        public string? Email { get; set; }
    }
}
