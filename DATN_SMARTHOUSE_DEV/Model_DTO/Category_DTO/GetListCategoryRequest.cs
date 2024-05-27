using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Category_DTO
{
    public class GetListCategoryRequest : BaseRequest
    {
        public string? Name { get; set; }
        public int? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

    }
}
