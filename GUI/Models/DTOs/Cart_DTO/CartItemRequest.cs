using GUI.FileBase;

namespace GUI.Models.DTOs.Cart_DTO
{
    public class CartItemRequest : BaseRequest
    {
        public List<Guid>? id { get; set; }
    }
}
