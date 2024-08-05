using System.ComponentModel.DataAnnotations;

namespace GUI.Models.DTOs.Order_DTO;

[Serializable]
public class OrderDetail
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public CustomerInfo Customer { get; set; } = null!;
    public ShippingInfo ShippingInfo { get; set; } = null!;
    public PaymentInfo PaymentInfo { get; set; } = null!;
    public bool IsCustomerTakeYourSelf { get; set; } = true;
    public bool IsSameAsCustomerAddress { get; set; } = true;
    public string StatusText { get; set; } = string.Empty;
    public int Status { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal VoucherDiscountAmount { get; set; }
    public decimal DiscountAmout { get; set; }
    public IList<OrderItem> Items { get; set; } = new List<OrderItem>();
    public DateTime TempOrderCreatedTime { get; set; }
    public bool IsDraft { get; set; }

    public void ReCalculatePaymentInfo()
    {
        PaymentInfo.TotalAmount = Items.Sum(e => e.Quantity * e.Price);
        PaymentInfo.TotalTax = PaymentInfo.TotalAmount * 10 / 100;
        PaymentInfo.FinalAmount = PaymentInfo.TotalAmount + PaymentInfo.TotalTax + PaymentInfo.ShippingFee - PaymentInfo.TotalDiscount;
    }
}

[Serializable]
public class OrderItem
{
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Guid Id { get; set; }
}

[Serializable]
public class CustomerInfo
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Chưa nhập tên khách hàng")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Chưa nhập số điện thoại")]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "Chưa nhập địa chỉ")]
    public string Address { get; set; } = string.Empty;
}

[Serializable]
public class ShippingInfo
{
    public bool IsCustomerTakeYourSelf { get; set; } = true;
    public bool IsSameAsCustomerAddress { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal ShippingFee { get; set; }
}