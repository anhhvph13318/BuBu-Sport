using GUI.FileBase;

namespace GUI.Models.Order_DTO
{
	public class OrderStatusRequest : BaseRequest
	{
        public Guid orderId { get; set; }
        public string orderCode { get; set; }
        public int paymentMethod { get; set; }
        public int paymentStatus { get; set; }
    }
}
