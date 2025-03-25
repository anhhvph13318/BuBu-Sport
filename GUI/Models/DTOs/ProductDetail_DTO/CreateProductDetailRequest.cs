using DATN_ACV_DEV.Model_DTO.Product_DTO;

namespace GUI.Models.DTOs.ProductDetail_DTO
{
    public class CreateProductDetailRequest
    {
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Guid? SizeName { get; set; }
        public Guid? Color { get; set; }
        public Guid? ProductID { get; set; }
    }
}
