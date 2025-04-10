using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.Model_DTO.ProductDetail_DTO;
using System.Text.Json.Serialization;

namespace GUI.Models.DTOs.Product_DTO
{
    public class DetailProductResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int? Status { get; set; }
        public Guid CategoryId { get; set; }

        public string? Description { get; set; }

        public decimal? PriceNet { get; set; }

        public string Image { get; set; }
        public string? UrlImage { get; set; }

        public bool? Vat { get; set; }

        public string? Warranty { get; set; }
        //public string? UrlImage { get; set; }

        public string? Color { get; set; }
        public string? SizeName { get; set; }

        public string? Material { get; set; }
        public List<Guid> PropertyID { get; set; }
        [JsonIgnore] // Ngăn việc serialize thuộc tính này
        public List<TbProductDetail> DetailData { get; set; }
        public List<TestDame> DetailDataFinal { get; set; }
        public List<string> ImageArray { get; set; }
        public string CategoryName { get; set; }
        public List<ProductDTO> RelatedProducts { get; set; }
    }
}
