namespace DATN_ACV_DEV.Model_DTO.Customer_DTO
{
    public class AccountCustomerResponse
    {
        public bool IsCustomer { get; set; }
        public AccountCustomerDTO Customer { get; set; }
    }
    public class AccountCustomerDTO
	{
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Adress { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public DateTime? YearOfBirth { get; set; }
        public int? Sex { get; set; }
    }
}
