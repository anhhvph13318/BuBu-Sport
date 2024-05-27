using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Image_DTO
{
    public class CreateImageRequest : BaseRequest
    {
        public string Url { get; set; }
        public string Type { get; set; }
        public Guid? ProductId { get; set; }
        public bool? InAcitve { get; set; } 

    }
}
