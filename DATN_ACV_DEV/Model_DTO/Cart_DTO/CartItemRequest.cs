using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Cart_DTO
{
    public class CartItemRequest : BaseRequest
    {
        public List<Guid>? id { get; set; }
        public Guid? colorId { get; set; }
        public Guid? sizeId { get; set; }
    }
}
