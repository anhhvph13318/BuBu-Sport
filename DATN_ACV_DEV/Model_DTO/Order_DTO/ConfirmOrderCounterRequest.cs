using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Order_DTO
{
    public class ConfirmOrderCounterRequest : BaseRequest
    {
        public int paymentMethodId { get; set; }
        public Guid? voucherID { get; set; }
        public string? voucherCode { get; set; }
        public string? description { get; set; }
        public List<Guid> cartDetailId { get; set; }
        public decimal? totalAmountDiscount { get; set; }
        public decimal? totalAmount { get; set; }
        public string? customerName { get; set; }
        public string? phoneNumber { get; set; }
    }
}
