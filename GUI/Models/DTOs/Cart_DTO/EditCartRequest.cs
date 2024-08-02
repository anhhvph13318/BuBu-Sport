using GUI.FileBase;

namespace GUI.Models.DTOs.Cart_DTO
{
    public class EditCartRequest : BaseRequest
    {
        public Guid CartDetaiID { get; set; }
        public int? Quantity { get; set; }
        public bool IsIncrement { get; set; }
    }
}
