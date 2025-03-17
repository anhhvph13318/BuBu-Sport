namespace GUI.Models.DTOs.ProductDetail_DTO
{
    public class GetListProductDetailResponse
    {
        public GetListProductDetailResponse()
        {
            LstProductDetail = new List<ProductDetailModel>();
        }
        public List<ProductDetailModel> LstProductDetail { get; set; }
    }
    public class ProductDetailModel
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
