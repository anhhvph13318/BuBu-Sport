using DATN_ACV_DEV.Model_DTO.Order_DTO;

namespace DATN_ACV_DEV.Model_DTO.Property_DTO
{
    public class GetListPropertyResponse
    {
        public GetListPropertyResponse()
        {
            Properties = new List<GetListPropertyDTO>();
        }
        public List<GetListPropertyDTO> Properties { get; set; }
        public int TotalCount { get; set; }
    }
    public class GetListPropertyDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
