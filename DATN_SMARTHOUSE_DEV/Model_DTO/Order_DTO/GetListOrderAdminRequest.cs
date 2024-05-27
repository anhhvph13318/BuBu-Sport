using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Order_DTO
{
    public class GetListOrderAdminRequest : BaseRequest
    {
        public string? code { get; set; }
        public int? status { get; set; }
        public bool? orderType { get; set; }
        public string? phoneNumberCustomer { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
    }
}
