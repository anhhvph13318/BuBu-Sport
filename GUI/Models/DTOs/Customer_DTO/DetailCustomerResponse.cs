namespace GUI.Models.DTOs.Customer_DTO
{
    public class DetailCustomerResponse : CreateCustomerRequest
    {
        public Guid Id { get; set; }
    }
    public class CustomerDTO 
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Adress { get; set; }

        public int? Rank { get; set; }

        public string? Status { get; set; }

        public DateTime? YearOfBirth { get; set; }

        public int? Sex { get; set; }

        public int? Point { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? UpdateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public Guid? CreateBy { get; set; }
        public Guid GroupCustomerId { get; set; }
    }
}
