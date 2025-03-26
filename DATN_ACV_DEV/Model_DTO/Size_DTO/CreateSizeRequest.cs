namespace DATN_ACV_DEV.Model_DTO.Size_DTO
{
    public class CreateSizeRequest
    {
        public Guid Id { get; set; }

        public string? SizeName { get; set; }

        public double? FootLength { get; set; }

        public int? Quantity { get; set; }
    }
}
