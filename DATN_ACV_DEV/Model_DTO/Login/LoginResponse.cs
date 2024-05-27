namespace DATN_ACV_DEV.Model_DTO.Login
{
    public class LoginResponse
    {
        public Guid Id { get; set; }
        public string Token { get; set; }        
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}
