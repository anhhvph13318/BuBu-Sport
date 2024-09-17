
namespace GUI.Models.Order_DTO
{
	public class OrderStatusResponse
	{
        public Guid orderId { get; set; }
        public int paymentMethod { get; set; }
        public int paymentStatus { get; set; }
    }
}
