using System.ComponentModel.DataAnnotations;

namespace GUI.Models.DTOs.Product_DTO
{
    public class DetailProductResponse
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        public string Name { get; set; }
        public string Code { get; set; }
        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]

        [Range(1, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Không được để trống số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Tên thương hiệu không được để trống.")]

        public string Brand { get; set; }
        public int? Status { get; set; }
        [Required(ErrorMessage = "Danh mục không được để trống")]

        public Guid CategoryId { get; set; }

        public string? Description { get; set; }
        [Required(ErrorMessage = "Giá bán không được để trống")]
        [Range(1, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0.")]
        public decimal? PriceNet { get; set; }

        public string Image { get; set; }
        public string? UrlImage { get; set; }

        public bool? Vat { get; set; }

        public string? Warranty { get; set; }
        //public string? UrlImage { get; set; }

        public string? Color { get; set; }

        public string? Material { get; set; }
        public List<Guid> PropertyID { get; set; }
    }
}
