using DATN_ACV_DEV.Model_DTO.Category_DTO;

namespace DATN_ACV_DEV.Model_DTO.Customer_DTO
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
