using GUI.FileBase;

namespace GUI.Models.Order_DTO
{
    public class OrderRequest : BaseRequest 
    {
        public string? description { get; set; }
        public List<Guid> cartDetailId { get; set; }
        public decimal? totalAmountDiscount { get; set; }
        public decimal? amountShip { get; set; }
        public decimal? totalAmount { get; set; }
        public string? addressDelivery { get; set; }
		public string? phoneNummber { get; set; }
		public string? name { get; set; }
		public Guid addressDeliveryId { get; set; }
        public int? paymentMethodId { get; set; }
        public bool getAtStore { get; set; }
        public List<Guid>? voucherID { get; set; }
        public List<string>? voucherCode { get; set; }
    }
}
