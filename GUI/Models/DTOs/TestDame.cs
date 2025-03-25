namespace GUI.Models.DTOs
{
    public class TestDame
    {
        public Guid Id { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public Guid? ImageId { get; set; }

        public Guid? ColorId { get; set; }

        public Guid? SizeId { get; set; }

        public Guid? ProductId { get; set; }

        public string SizeName { get; set; }
        public string ColorName { get; set; }

    }
}
