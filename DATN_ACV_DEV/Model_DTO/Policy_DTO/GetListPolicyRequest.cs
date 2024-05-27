using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Policy_DTO
{
    public class GetListPolicyRequest : BaseRequest
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Type { get; set; }
    }
}
