namespace DATN_ACV_DEV.Model_DTO.Account_DTO
{
    public class DetailAccountResponse
    {
        public Guid Id { get; set; }

        public string? AccountCode { get; set; }

        public string? Email { get; set; }

        public string Password { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }

        public Guid? CustomerID { get; set; }
    }
}
