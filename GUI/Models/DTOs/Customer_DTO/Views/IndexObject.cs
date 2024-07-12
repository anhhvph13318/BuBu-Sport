namespace GUI.Models.DTOs.Customer_DTO.Views
{
    public class IndexObject
    {
        public GetListCustomerResponse Data { get; set; } = new();
        public CustomerDTO Model { get; set; } = new();
        public string Search { get; set; } = string.Empty;
    }
}
