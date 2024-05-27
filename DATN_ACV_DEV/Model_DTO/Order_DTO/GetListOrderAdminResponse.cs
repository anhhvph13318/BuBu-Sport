using System.Reflection.Metadata;

namespace DATN_ACV_DEV.Model_DTO.Order_DTO
{
    public class GetListOrderAdminResponse
    {
        public GetListOrderAdminResponse()
        {
            Orders = new List<GetListOrderAdminDTO>();
        }
        public List<GetListOrderAdminDTO> Orders { get; set; }
        public int TotalCount { get; set; }
    }
    public class GetListOrderAdminDTO
    {
        public Guid id { get; set; }
        public string? code { get; set; }
        public string? description { get; set; }
        public string status { get; set; }
        public string? nameCustomer { get; set; }
        public string paymentMethodName { get; set; }
        public decimal? amountShip { get; set; }
        public decimal? amountDiscount { get; set; }
        public decimal? totalAmount { get; set; }
        public string? products { get; set; }
        public string? ReasionCancel { get; set; }
        public List<string>? ImageForCancelOrder { get; set; }
    }
}
