
namespace GUI.Models.DTOs.Cart_DTO
{
    public class CartItemResponse
    {
        public CartItemResponse()
        {
            CartItem = new List<CartDTO>();
        }
        public List<CartDTO> CartItem { get; set; }
    }
}
