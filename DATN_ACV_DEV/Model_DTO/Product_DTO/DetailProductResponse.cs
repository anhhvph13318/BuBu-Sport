using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.Model_DTO.ProductDetail_DTO;
using DATN_ACV_DEV.Models;
using System.Text.Json.Serialization;

namespace DATN_ACV_DEV.Model_DTO.Product_DTO
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
        public List<string> ImageArray { get; set; }
        public string CategoryName { get; set; }
        public List<DATN_ACV_DEV.Entity.TbProduct> RelatedProducts { get; set; }
    }
}
