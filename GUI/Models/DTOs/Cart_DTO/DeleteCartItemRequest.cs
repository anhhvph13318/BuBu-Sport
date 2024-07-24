using GUI.FileBase;

namespace GUI.Models.DTOs.Cart_DTO
{
    public class DeleteCartItemRequest : BaseRequest 
    {
        public Guid Id { get; set; }
    }
}
