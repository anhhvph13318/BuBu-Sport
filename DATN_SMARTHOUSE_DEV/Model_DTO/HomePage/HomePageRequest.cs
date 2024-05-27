using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.HomePage
{
    public class HomePageRequest : BaseRequest
    {
        public string? Name { get; set; }
        public Guid? CategoryID { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public int? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? OffSet { get; set; } = 0;
        public int? Limit { get; set; }
    }
}
