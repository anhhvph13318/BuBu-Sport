using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Order_DTO
{
    public class CreateOrderRequest : BaseRequest
    {
        public List<Guid> cartDetailID { get; set; }
        public string? description { get; set; }
        public List<Guid>? voucherID { get; set; }
        public Guid? paymentMenthodID { get; set; }
        public Guid? AddressDeliveryId { get; set; }
        public string TokenGHN { get; set; }

    }
    public class OrderProduct
    {
        public Guid productId { get; set; }
        public Guid categoryId { get; set; }
        public string productName { get; set; }
        public string productCode { get; set; }
        public decimal? price { get; set; }
        public int? weight { get; set; }
        public string? url { get; set; }
        public int quantity { get; set; }

    }
    public class OrderDetailProduct
    {
        public Guid orderId { get; set; }
        public Guid productId { get; set; }
        public string productName { get; set; }
        public decimal price { get; set; }
        public string url { get; set; }
        public int quantity { get; set; }

    }
}
