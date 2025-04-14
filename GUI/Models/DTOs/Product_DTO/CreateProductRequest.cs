namespace GUI.Models.DTOs.Product_DTO
{
    public class CreateProductRequest
    {
        public string? Name { get; set; }
        public string? Code { get; set; }

        public decimal Price { get; set; }

        public int? Status { get; set; }

        public string? Description { get; set; }

        public decimal? PriceNet { get; set; }
        public string? SizeName { get; set; }

        public bool? Vat { get; set; }

        public string? Warranty { get; set; }

        public Guid? Color { get; set; }

        public string? Material { get; set; }
        public int Quantity {  get; set; }

        public Guid? ImageId { get; set; }
        public string UrlImage { get; set; }
        public string? TypeImage { get; set; }
        public Guid CategoryId { get; set; }
        public List<string>? OpenAttribute { get; set; }
        public List<Guid>? PropertyID { get; set; }
        public string? TypeEditProperty { get; set; }
        public List<SizeQuantityDto> SizesQuantities { get; set; }

    }
    public class SizeQuantityDto
    {
        public Guid IdSize { get; set; } // ID của Size
        public int QuantitySize { get; set; } // Số lượng
    }
}
