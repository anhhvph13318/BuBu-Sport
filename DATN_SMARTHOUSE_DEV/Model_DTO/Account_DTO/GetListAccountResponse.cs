namespace DATN_ACV_DEV.Model_DTO.Account_DTO
{
    public class GetListAccountResponse
    {
        public GetListAccountResponse()
        {
            LstAccount = new List<AccountDTO> { };
        }
        public List<AccountDTO> LstAccount { get; set; }
        public int TotalCount { get; set; }
    }
    public class AccountDTO 
    {
        public Guid Id { get; set; }

        public string? AccountCode { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }

        public Guid CustomerID { get; set; }
    }
}
