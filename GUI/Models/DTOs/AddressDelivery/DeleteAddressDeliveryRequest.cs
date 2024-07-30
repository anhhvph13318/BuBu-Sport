using GUI.FileBase;
using System.ComponentModel.DataAnnotations;

namespace GUI.Models.DTOs.Address
{
    public class DeleteAddressDeliveryRequest : BaseRequest
    {
        [Required]
        public Guid id { get; set; }
    }
}
