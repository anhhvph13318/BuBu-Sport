using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Cart_DTO
{
    public class EditCartRequest : BaseRequest
    {
        public Guid CartDetaiID { get; set; }
        public int? Quantity { get; set; }

    }
}
