using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Cart_DTO
{
    public class DeleteCartItemRequest : BaseRequest 
    {
        public Guid Id { get; set; }
    }
}
