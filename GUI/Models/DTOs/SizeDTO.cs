namespace GUI.Models.DTOs
{
    public class SizeDTO
    {
        public Guid Id { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public string FootLength { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
