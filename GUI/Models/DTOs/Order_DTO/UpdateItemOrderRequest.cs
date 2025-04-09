namespace GUI.Models.DTOs.Order_DTO
{
    public class UpdateItemOrderRequest
    {
        public IList<OrderItem> Items { get; set; }
        public int Status { get; set; }
    }
}
