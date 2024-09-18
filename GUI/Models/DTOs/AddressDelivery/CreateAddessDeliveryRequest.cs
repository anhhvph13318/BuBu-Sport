using GUI.FileBase;
using System.ComponentModel.DataAnnotations;

namespace GUI.Models.DTOs.Address
{
    public class CreateAddessDeliveryRequest : BaseRequest
    {
        [Required]
        public string provinceName { get; set; }
        [Required]
        public int provinceId { get; set; }
        [Required]
        public string districName { get; set; }
        [Required]
        public int districId { get; set; }
        [Required]
        public string wardName { get; set; }
        [Required]
        public bool status { get; set; }

        [Required]
        public string receiverName { get; set; }
         [Required]
        public string receiverPhone { get; set; }
    }
}
