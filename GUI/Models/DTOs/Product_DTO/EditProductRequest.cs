namespace GUI.Models.DTOs.Product_DTO
{
    public class EditProductRequest 
    {
        public Guid ID { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string? Description { get; set; }
        public string? UrlImage { get; set; }
    }
}
