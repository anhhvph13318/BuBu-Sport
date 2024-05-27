using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Voucher_DTO
{
    public class GetListVoucherRequest : BaseRequest
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Type { get; set; }
        public string? Unit { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? GroupCustomerId { get; set; }
    }
}
