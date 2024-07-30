namespace DATN_ACV_DEV.Model_DTO.Order_DTO;

public class OrderDetail
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public string ShippingAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string Status { get; set; }
    public string PaymentMethodName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal VoucherDiscountAmount { get; set; }
    public decimal DiscountAmout { get; set; }
    public IEnumerable<OrderItem> Items { get; set; }
}

public class OrderItem
{
    public string ProductName { get; set; }
    public string ProductImage { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Guid Id { get; set; }
}