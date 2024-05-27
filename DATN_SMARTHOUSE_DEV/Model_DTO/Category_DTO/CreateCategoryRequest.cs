namespace DATN_ACV_DEV.Model_DTO.Category_DTO
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public  Guid? ImageId { get; set; }
        public List<string>? UrlImage { get; set; }
        public string? TypeImage { get; set; }
    }
}
