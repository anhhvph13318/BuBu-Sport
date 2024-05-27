namespace DATN_ACV_DEV.Model_DTO.Property_DTO
{
    public class CreatePropertyRequest
    {
        public string Name { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
