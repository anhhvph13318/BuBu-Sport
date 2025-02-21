using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbOrder
{
    public Guid Id { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public Guid? UpdateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? AccountId { get; set; }

    public int PaymentMethod { get; set; }

    public decimal? AmountShip { get; set; }

    public decimal? TotalAmountDiscount { get; set; }

    public string? OrderCode { get; set; }

    public string? OrderCodeGhn { get; set; }

    public Guid? AddressDeliveryId { get; set; }

    public bool? OrderCounter { get; set; }

    public string? ReasionCancel { get; set; }

    public string? PhoneNumberCustomer { get; set; } // SDT Khách hàng
    public decimal? Discount { get; set; } // thêm
    public decimal SubTotal { get; set; } // thêm
    public string? CustomerName { get; set; } // Tên khách hàng
    public string? AddressCustomer { get; set; } // Địa chỉ khách
    public Guid? EmployeeId { get; set; } // ID nhân viên xử lý
    public short PaymentStatus { get; set; }

    public bool IsCustomerTakeYourself { get; set; }

    public bool IsShippingAddressSameAsCustomerAddress { get; set; }

    public bool IsDraft { get; set; }

    public Guid? VoucherId { get; set; }

    public virtual TbAddressDelivery? AddressDelivery { get; set; }

    public virtual TbCustomer? Customer { get; set; }

    public virtual ICollection<TbOrderDetail> TbOrderDetails { get; set; } = new List<TbOrderDetail>();

    public virtual TbVoucher? Voucher { get; set; }
}
