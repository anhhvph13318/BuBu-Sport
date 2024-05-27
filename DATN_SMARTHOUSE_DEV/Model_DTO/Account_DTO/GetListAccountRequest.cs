using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Account_DTO
{
    public class GetListAccountRequest : BaseRequest
    {
        public string? AccountCode { get; set; } 

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
    } 
}
