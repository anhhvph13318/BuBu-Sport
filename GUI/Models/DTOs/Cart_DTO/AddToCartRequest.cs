using GUI.FileBase;

namespace GUI.Models.DTOs.Cart_DTO
{
    public class AddToCartRequest : BaseRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Type { get; set; }
        public bool incre = true;
        public Guid ColorId { get; set; }  // 🆕 Thêm màu sắc
        public Guid SizeId { get; set; }   // 🆕 Thêm kích thước
    }
}
