using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Order")]
public partial class TbOrder
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? TotalAmount { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public Guid? UpdateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [Column("VoucherID")]
    public Guid? VoucherId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? AccountId { get; set; }

    public int? PaymentMethod { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? AmountShip { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? TotalAmountDiscount { get; set; }

    public string? OrderCode { get; set; }

    public string? OrderCodeGhn { get; set; }

    public Guid? AddressDeliveryId { get; set; }

    public bool? OrderCounter { get; set; }

    public string? ReasionCancel { get; set; }

    public string? PhoneNumberCustomer { get; set; }

    public string? AddressCustomer { get; set; }

    public bool? IsCustomerTakeYourself { get; set; }

    public bool? IsShippingAddressSameAsCustomerAddress { get; set; }

    public bool? IsDraft { get; set; }

    [ForeignKey("VoucherId")]
    [InverseProperty("TbOrders")]
    public virtual TbVoucher? Voucher { get; set; }
}
