using DATN_ACV_DEV.FileBase;
using System.ComponentModel.DataAnnotations;

namespace DATN_ACV_DEV.Model_DTO.AddressDelivery
{
    public class CreateAddessDeliveryRequest : BaseRequest
    {
        [Required]
        public string provinceName { get; set; }
        public int? provinceId { get; set; }
        [Required]
        public string districName { get; set; }
        public int? districId { get; set; }
        [Required]
        public string wardName { get; set; }
        public string? wardCode { get; set; }
        [Required]
        public bool status { get; set; }

        [Required]
        public string receiverName { get; set; }
         [Required]
        public string receiverPhone { get; set; }
    }
}
