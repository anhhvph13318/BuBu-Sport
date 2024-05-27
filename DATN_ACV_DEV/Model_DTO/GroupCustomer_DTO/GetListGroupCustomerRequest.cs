using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.GroupCustomer_DTO
{
    public class GetListGroupCustomerRequest : BaseRequest
    {
        public string? Name { get; set; } = null!;
    }
} 
