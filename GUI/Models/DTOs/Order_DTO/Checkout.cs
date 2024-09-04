namespace GUI.Models.DTOs.Order_DTO
{
    public class Checkout
    {
        public CustomerInfo CustomerInfo { get; set; } = new CustomerInfo();
        public ShippingInfo ShippingInfo { get; set; } = new ShippingInfo();
        public bool IsShippingAddressSameAsCustomerAddress { get; set; }
        public bool IsCustomerTakeYourSelf { get; set; }
        public int Status { get; set; }
        public bool IsDraft { get; set; }
    }
}
