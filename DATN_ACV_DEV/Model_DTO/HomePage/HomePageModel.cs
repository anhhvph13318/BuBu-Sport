namespace DATN_ACV_DEV.Model_DTO.HomePage
{
    public class HomePageModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? status { get; set; }
        public decimal? PriceNet { get; set; }
        public string? Description { get; set; }
        public string Image { get; set; }
        

    }
}
