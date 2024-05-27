namespace DATN_ACV_DEV.Model_DTO.Order_DTO
{
    public class OrderResponse : CreateOrderResponse 
    {
        public Guid id { get; set; }
        public string? orderCode { get; set; }
        public string? voucherCode { get; set; }
        public string? accountCode { get; set; }
        public string? nameCustomer { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime createdDate { get; set; }
        public string? PaymentMethodName { get; set; }
    }
}
