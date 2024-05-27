namespace DATN_ACV_DEV.Model_DTO.GroupCustomer_DTO
{
    public class CreateGroupCustomerRequest
    {
        public string Name { get; set; }

        public int MinPoint { get; set; }

        public int MaxPoint { get; set; }

        public bool? IsDelete { get; set; }
    }
} 
