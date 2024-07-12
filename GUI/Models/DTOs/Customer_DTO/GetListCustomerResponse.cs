
namespace GUI.Models.DTOs.Customer_DTO
{
    public class GetListCustomerResponse
    {
        public GetListCustomerResponse()
        {
            LstCustomer = new List<CustomerDTO>();
        }
        public List<CustomerDTO> LstCustomer { get; set; }
        public int TotalCount { get; set; }
       
    }
} 
