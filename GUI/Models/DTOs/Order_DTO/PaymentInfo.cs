namespace GUI.Models.DTOs.Order_DTO
{
    public class PaymentInfo
    {
        public Guid? VoucherId { get; set; }
        public string VoucherCode { get; set; } = "";
        public int Status { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal FinalAmount { get; set; }
        public string[] Products { get; set; } = Array.Empty<string>();
    }
}
