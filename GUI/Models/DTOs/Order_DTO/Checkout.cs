namespace GUI.Models.DTOs.Order_DTO
{
    public class Checkout
    {
        public CustomerInfo CustomerInfo { get; set; } = new CustomerInfo();
        public ShippingInfo ShippingInfo { get; set; } = new ShippingInfo();
        public string Code { get; set; } = string.Empty;
        public bool IsShippingAddressSameAsCustomerAddress { get; set; }
        public bool IsCustomerTakeYourSelf { get; set; }
        public int Status { get; set; }
    }
}
