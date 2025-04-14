using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Order_DTO
{
    public class UpdatStatusOrderRequest : BaseRequest
    {
        public Guid id { get; set; }
        public int status { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string phone { get; set; }
        public string statusText { get; set; }
        public IList<OrderItem> products { get; set; }
    }
}
