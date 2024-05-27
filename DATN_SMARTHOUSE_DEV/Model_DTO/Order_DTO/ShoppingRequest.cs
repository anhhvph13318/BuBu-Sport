using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Order_DTO
{
    public class ShoppingRequest
    {
        public Guid productId { get; set; }
        public int quantity { get; set; }
        public string phoneNumber { get; set; }
        public decimal amountShip { get; set; } = 0;
        public string address { get; set; }
        public Guid paymentMethodId { get; set; }
    }
}
