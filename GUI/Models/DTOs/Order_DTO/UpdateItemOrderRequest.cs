namespace GUI.Models.DTOs.Order_DTO
{
    public class UpdateItemOrderRequest
    {
        public IList<DATN_ACV_DEV.Model_DTO.Order_DTO.OrderItem> Items { get; set; }
        public int Status { get; set; }
        public int paymentMethod { get; set; }
        public CustomerInfo CustomerInfo { get; set; } = new CustomerInfo();

    }
}
