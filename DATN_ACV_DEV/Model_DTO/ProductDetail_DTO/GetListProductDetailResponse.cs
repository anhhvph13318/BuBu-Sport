using DATN_ACV_DEV.Entity;

namespace DATN_ACV_DEV.Model_DTO.ProductDetail_DTO
{
	public class GetListProductDetailResponse
	{
		public GetListProductDetailResponse() 
		{
			LstProduct = new List<ProductDetailDTO>();

		}
		public List<ProductDetailDTO> LstProduct { get; set; }
		public int TotalCount { get; set; }
	}
	public class ProductDetailDTO
	{
		public Guid? Id { get; set; }
		public string Code { get; set; }
		public string Image { get; set; }
		public string Name { get; set; }
		public string Color { get; set; }
		public string Size { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public string TotalAmount { get; set; }
	}
}
