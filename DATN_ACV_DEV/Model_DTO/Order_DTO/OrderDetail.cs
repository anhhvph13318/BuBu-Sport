using DATN_ACV_DEV.Model_DTO.Voucher_DTO;

namespace DATN_ACV_DEV.Model_DTO.Order_DTO;

public class OrderDetail
{
    public Guid Id { get; set; }
    public CustomerInfo Customer { get; set; } = null!;
    public ShippingInfo? ShippingInfo { get; set; } = null!;
    public PaymentInfo PaymentInfo { get; set; } = null!;
    public VoucherDTO Voucher { get; set; } = new VoucherDTO();
    public bool IsCustomerTakeYourSelf { get; set; } = true;
    public bool IsSameAsCustomerAddress { get; set; } = true;
    public string StatusText { get; set; }
    public int Status { get; set; }
    public string PaymentMethodName { get; set; }
    public IEnumerable<OrderItem> Items { get; set; }
    public string Code { get; set; } = string.Empty;
    public string OrderTypeName { get; set; } = string.Empty;
    public bool IsDraft { get; set; }
    public int PaymentMethod { get; set; }
}

public class OrderItem
{
    public string ProductName { get; set; }
    public string ProductImage { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Guid Id { get; set; }
}

public class ShippingInfo
{
    public static ShippingInfo Default()
    {
        return new ShippingInfo
        {
            Name = "",
            PhoneNumber = "",
            Address = ""
        };
    }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal ShippingFee { get; set; }
}

public class CustomerInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class PaymentInfo
{
    public Guid? VoucherId { get; set; }
    public string VoucherCode { get; set; } = string.Empty;
    public int Status { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal FinalAmount => TotalAmount + ShippingFee + TotalTax - TotalDiscount;
}