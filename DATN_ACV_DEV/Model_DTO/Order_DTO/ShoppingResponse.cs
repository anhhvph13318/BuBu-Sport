namespace DATN_ACV_DEV.Model_DTO.Order_DTO
{
    public class ShoppingResponse
    {
        public Guid id { get; set; }
        public string orderCode { get; set; }
        public decimal? totalAmount { get; set; }
        public decimal amountShip { get; set; }
        public OrderProduct product { get; set; }
    }
}
