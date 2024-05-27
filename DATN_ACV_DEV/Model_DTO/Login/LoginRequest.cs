using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Login
{
    public class LoginRequest : BaseRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
