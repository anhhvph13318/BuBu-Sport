using GUI.FileBase;

namespace GUI.Models.DTOs.Cart_DTO
{
    public class AddToCartRequest : BaseRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Type { get; set; }
        public bool incre = true;
    }
}
