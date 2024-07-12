namespace GUI.Models.DTOs.Product_DTO
{
    public class GetListProductResponse 
    {
        
        public GetListProductResponse()
        {
            LstProduct = new List<ProductModel>();
        }
        public List<ProductModel> LstProduct { get; set; }
        public int TotalCount { get; set; }
    }
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Status { get; set; }
        public decimal? PriceNet { get; set; }
        public string? Description { get; set; }
        public string Image { get; set; }
        public string Code { get; set; }
    }
}
