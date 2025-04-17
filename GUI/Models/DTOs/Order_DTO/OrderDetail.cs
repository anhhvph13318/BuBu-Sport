using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.Model_DTO.GHN_DTO;
using GUI.Models.DTOs.Voucher_DTO;
using System.ComponentModel.DataAnnotations;
using static DATN_ACV_DEV.Controllers.Order.AdminCreateOrderController;
using System.Globalization;

namespace GUI.Models.DTOs.Order_DTO;

[Serializable]
public class OrderDetail
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public CustomerInfo Customer { get; set; } = null!;
    public ShippingInfo ShippingInfo { get; set; } = null!;
    public PaymentInfo PaymentInfo { get; set; } = null!;
    public VoucherDTO Voucher { get; set; } = new VoucherDTO();
    public bool IsCustomerTakeYourSelf { get; set; } = true;
    public bool IsSameAsCustomerAddress { get; set; } = true;
    public string StatusText { get; set; } = string.Empty;
    public int Status { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public int PaymentMethod { get; set; }
    public short PaymentStatus { get; set; }
    public IList<DATN_ACV_DEV.Model_DTO.Order_DTO.OrderItem> Items { get; set; } = new List<DATN_ACV_DEV.Model_DTO.Order_DTO.OrderItem>();
    public DateTime TempOrderCreatedTime { get; set; }
    public DateTime Created { get; set; } //VANH
    public bool IsDraft { get; set; }
    public string OrderTypeName { get; set; } = string.Empty;
    public bool AllowRemove()
    {
        if (IsDraft) return true;

        if (Code.StartsWith("OFF")) return false;

        return true;
    }
    public void ReCalculatePaymentInfo()
    {
        PaymentInfo.TotalAmount = Items.Sum(e => e.Quantity * e.Price);
        PaymentInfo.FinalAmount = PaymentInfo.TotalAmount + PaymentInfo.ShippingFee;
        PaymentInfo.Products = Items.Select(e => $"{e.ProductName} - {e.Price.ToString("C", CultureInfo.GetCultureInfo("vi-VN"))}").ToArray();


        Console.WriteLine($"TotalAmount: {PaymentInfo.TotalAmount}, Voucher.Condition: {Voucher?.Condition}");
        if (Voucher == null || Voucher.Id == Guid.Empty)
        {
            Console.WriteLine("Voucher is null or empty");
            return;
        }
        if (PaymentInfo.TotalAmount < Voucher.Condition)
        {
            Console.WriteLine("TotalAmount is less than Voucher.Condition");
            Voucher = new VoucherDTO();
            PaymentInfo.VoucherId = Guid.Empty;
            PaymentInfo.VoucherCode = string.Empty;
            return;
        }
        if (Voucher != null && Voucher.Unit == VoucherUnit.Percent)
        {
            var discount = PaymentInfo.TotalAmount * Voucher.Discount / 100;
            var finalDiscount = discount > Voucher.MaxDiscount
            ? Voucher.MaxDiscount
                : discount;
            PaymentInfo.TotalDiscount = finalDiscount;
            PaymentInfo.FinalAmount -= finalDiscount;
        }
        else
        {
            PaymentInfo.TotalDiscount = Voucher != null ? Voucher.Discount : PaymentInfo.TotalDiscount;
            PaymentInfo.FinalAmount -= Voucher != null ? Voucher.Discount : 0;
        }
    }
}

[Serializable]
public class OrderItem
{
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Code { get; set; } = string.Empty; 
    public Guid Id { get; set; }
}


public class CustomerInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

[Serializable]
public class ShippingInfo
{
    public bool IsCustomerTakeYourSelf { get; set; } = true;
    public bool IsSameAsCustomerAddress { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

[Serializable]
public record Stock(Guid Id, int Quantity);

public record PaymentMethod(int Method, bool IsEdit);