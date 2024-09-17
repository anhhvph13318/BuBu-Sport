namespace GUI.Models.DTOs.Order_DTO
{
    public class PaymentInfo
    {
        public bool IsCustomerTakeYourSelf { get; set; } = true;
        public Guid? VoucherId { get; set; }
        public string VoucherCode { get; set; } = "";
        public int Status { get; set; } = -1;
        public decimal ShippingFee { get; private set; } = 30000;
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal FinalAmount { get; set; }
        public string[] Products { get; set; } = Array.Empty<string>();
    }
}
