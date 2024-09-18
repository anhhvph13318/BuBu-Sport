using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Product_DTO
{
    public class CreateProductRequest : BaseRequest
    {
        public string? Name { get; set; }
        public string? Code { get; set; }

        public decimal Price { get; set; }

        public int? Status { get; set; }
        public int Quantity { get; set; }

        public string? Description { get; set; }

        public decimal? PriceNet { get; set; }

        public bool? Vat { get; set; }

        public string? Warranty { get; set; }

        public string? Color { get; set; }

        public string? Material { get; set; }

        public Guid? ImageId { get; set; }
        public string? UrlImage { get; set; }
        public string? TypeImage { get; set; }
        public Guid CategoryId { get; set; }
        public List<string>? OpenAttribute { get; set; }
        public List<Guid>? PropertyID { get; set; }
        public string? TypeEditProperty { get; set; }


    }
}
