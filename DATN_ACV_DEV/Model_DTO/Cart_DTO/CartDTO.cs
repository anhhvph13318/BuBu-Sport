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

    }
}
