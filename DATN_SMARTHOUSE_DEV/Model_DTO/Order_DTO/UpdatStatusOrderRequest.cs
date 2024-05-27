using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Order_DTO
{
    public class UpdatStatusOrderRequest : BaseRequest
    {
        public Guid id { get; set; }
        public int status { get; set; }
    }
}
