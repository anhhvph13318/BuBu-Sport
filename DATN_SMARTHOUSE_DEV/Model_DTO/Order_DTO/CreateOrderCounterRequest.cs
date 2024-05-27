using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Order_DTO
{
    public class CreateOrderCounterRequest : BaseRequest
    {
        public List<Guid> cartDetailID { get; set; }
        public string? description { get; set; }
        public string? voucherCode { get; set; }
        public Guid? paymentMenthodID { get; set; }
        
    }
}
