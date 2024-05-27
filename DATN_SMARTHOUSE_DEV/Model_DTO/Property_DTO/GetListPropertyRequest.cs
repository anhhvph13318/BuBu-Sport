using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Property_DTO
{
    public class GetListPropertyRequest: BaseRequest
    {
        public string? Name { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
