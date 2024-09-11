using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Customer_DTO
{
    public class CreateCustomerRequest: BaseRequest
    {
        public string? Name { get; set; }

        public string? Adress { get; set; }

        public int? Rank { get; set; }

        public string? Status { get; set; }

        public DateTime? YearOfBirth { get; set; }

        public int? Sex { get; set; }

        public string? Phone { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? UpdateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public Guid? CreateBy { get; set; }

        public Guid GroupCustomerId { get; set; }
    }
}
