using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Order_DTO
{
	public class OrderStatusRequest : BaseRequest
	{
        public Guid orderId { get; set; }
		public string orderCode { get; set; }
		public int paymentMethod { get; set; }
        public int paymentStatus { get; set; }
    }
}
