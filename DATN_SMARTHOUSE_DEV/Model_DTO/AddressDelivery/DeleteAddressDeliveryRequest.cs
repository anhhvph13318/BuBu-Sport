using DATN_ACV_DEV.FileBase;
using System.ComponentModel.DataAnnotations;

namespace DATN_ACV_DEV.Model_DTO.AddressDelivery
{
    public class DeleteAddressDeliveryRequest : BaseRequest
    {
        [Required]
        public Guid id { get; set; }
    }
}
