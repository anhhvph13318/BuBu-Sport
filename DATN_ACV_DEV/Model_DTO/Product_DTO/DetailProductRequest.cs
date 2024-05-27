using DATN_ACV_DEV.Entity;

namespace DATN_ACV_DEV.Model_DTO.Product_DTO
{
    public class DetailProductRequest: DeleteProduct
    {       
    }
    public class ProductDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int? Status { get; set; }

        public string? Description { get; set; }

        public decimal? PriceNet { get; set; }

        public string Image { get; set; }

        public string CategoryName { get; set; }

        public string Vat { get; set; }

        public string? Warranty { get; set; }

        public string? Color { get; set; }

        public string? Material { get; set; }

    }

}
