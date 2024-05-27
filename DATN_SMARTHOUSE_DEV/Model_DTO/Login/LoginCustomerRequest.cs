using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Login
{
    public class LoginCustomerRequest: BaseRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
