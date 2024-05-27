namespace DATN_ACV_DEV.Model_DTO.Category_DTO
{
    public class DetailCategoryResponse
    {
        public DetailCategoryResponse()
        {
            category = new CategoryDTO();
        }
        public CategoryDTO category { get; set; }   
    }
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Image { get; set; }
    }
}
