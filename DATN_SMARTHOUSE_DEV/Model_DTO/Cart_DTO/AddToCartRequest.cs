using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Cart_DTO
{
    public class AddToCartRequest : BaseRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Type { get; set; }

    }
}
