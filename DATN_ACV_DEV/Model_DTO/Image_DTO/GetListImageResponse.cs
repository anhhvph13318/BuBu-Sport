using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Image_DTO
{
    public class GetListImageResponse 
    {
        public GetListImageResponse()
        {
            LstImage = new List<GetListImageDTO>();
        }
        public List<GetListImageDTO> LstImage { get; set; }    
        public int TotalCount { get; set; }
      
    }
    public class GetListImageDTO
    {
        public Guid Id { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; }
        public Guid? ProductId { get; set; }
        public bool? InAcitve { get; set; }
    }
}
