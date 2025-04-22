namespace GUI.Models.Order_DTO
{
    public class CreateOrderResponse
    {
        public CreateOrderResponse()
        {
            products = new List<DATN_ACV_DEV.Model_DTO.Order_DTO.OrderProduct>();
        }
        public List<DATN_ACV_DEV.Model_DTO.Order_DTO.OrderProduct> products { get; set; }
        public List<Guid> cartDetailId { get; set; }
        public decimal? totalAmountDiscount { get; set; }
        public decimal? amountShip { get; set; }
        public string? addressDelivery { get; set; }
        public Guid addressDeliveryId { get; set; }
        public decimal? totalAmount { get; set; }
        public Guid? paymentMethodId { get; set; }
        public List<string>? VoucherCodes { get; set; }
        public List<Guid>? VoucherId { get; set; }
        public string? description { get; set; }

    }
}
