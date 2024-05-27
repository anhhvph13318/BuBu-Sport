using DATN_ACV_DEV.Entity;

namespace DATN_ACV_DEV.Model_DTO.Cart_DTO
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
