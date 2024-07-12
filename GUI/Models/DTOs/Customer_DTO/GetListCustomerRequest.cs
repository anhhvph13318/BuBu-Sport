using GUI.FileBase;

namespace GUI.Models.DTOs.Customer_DTO
{
    public class GetListCustomerRequest : BaseRequest
    {
        public string? Name { get; set; }
        public string? Rank { get; set; }
        public string? Status { get; set; }
        public string? Sex { get; set; }
    }
} 
