namespace DATN_ACV_DEV.Model_DTO.GroupCustomer_DTO
{
    public class GetListGroupCustomerResponse
    {
        public  GetListGroupCustomerResponse() 
        {
            LstGroupCustomer = new List<GroupCustomerDTO> { };
        }
        public List<GroupCustomerDTO> LstGroupCustomer { get; set; }
        public int TotalCount { get; set; }
    }
    public class GroupCustomerDTO 
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public int MinPoint { get; set; }

        public int MaxPoint { get; set; }

    }
}
