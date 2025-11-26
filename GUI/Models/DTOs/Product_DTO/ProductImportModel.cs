namespace GUI.Models.DTOs.Product_DTO
{
    public class ProductImportModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string CategoryId { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string UrlImage { get; set; }


        public int XS { get; set; }
        public int S { get; set; }
        public int M { get; set; }
        public int L { get; set; }
        public int XL { get; set; }
        public int XXL { get; set; }
    }
}
