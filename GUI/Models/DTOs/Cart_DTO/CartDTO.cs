namespace GUI.Models.DTOs.Cart_DTO
{
    public class CartDTO
    {
        public Guid CartDetailID { get; set; }
        public Guid ProductID { get; set; }
        public string NameProduct { get; set; }
        public string Image { get; set;}
        public string Color { get; set;}
        public string Size { get; set;}
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
