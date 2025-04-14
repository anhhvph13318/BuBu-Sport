namespace DATN_ACV_DEV.Model_DTO.Cart_DTO
{
    public class CartDTO
    {
        public Guid CartDetailID { get; set; }
        public Guid ProductID { get; set; }
        public string NameProduct { get; set; }
        public string ProductCode { get; set; }
        public string Image { get; set;}
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public string? SizeName { get; set; }
        public string? receiverName { get; set; }
        public string? receiverEmail { get; set; }
        public string? receiverPhone { get; set; }
        public string? receiverDistrict { get; set; }
        public string? receiverWard { get; set; }
        public string? receiverProvince { get; set; }

    }
}
