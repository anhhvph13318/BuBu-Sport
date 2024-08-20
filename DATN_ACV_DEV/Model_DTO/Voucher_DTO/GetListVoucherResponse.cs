using DATN_ACV_DEV.Entity;

namespace DATN_ACV_DEV.Model_DTO.Voucher_DTO
{
    public class GetListVoucherResponse
    {
        public GetListVoucherResponse()
       {
            LstVoucher = new List<VoucherDTO>();
        }
        public List<VoucherDTO> LstVoucher { get; set; }
        public int TotalCount { get; set; }
    }
    public class VoucherDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Discount { get; set; }
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public VoucherType Type { get; set; } = VoucherType.Voucher;
        public VoucherUnit Unit { get; set; }
        public bool Status { get; set; }
        public decimal MaxDiscount { get; set; }
        public decimal RequiredTotalAmount { get; set; }
    }
}
