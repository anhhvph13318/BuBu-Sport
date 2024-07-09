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

        public string? Description { get; set; }

        public decimal? PriceNet { get; set; }

        public string Image { get; set; }

        public bool? Vat { get; set; }

        public string? Warranty { get; set; }

        public string? Color { get; set; }

        public string? Material { get; set; }
        public List<Guid> PropertyID { get; set; }
    }
}
