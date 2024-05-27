namespace DATN_ACV_DEV.Model_DTO.Category_DTO
{
    public class GetListCategoryResponse
    {
        public GetListCategoryResponse()
        {
            LstCategory = new List<CategoryDTO>();
        }
        public List<CategoryDTO> LstCategory { get; set; }
        public int TotalCount { get; set; }
    }
}
