using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Order")]
[Index("AddressDeliveryId", Name = "IX_tb_Order_AddressDeliveryId")]
[Index("CustomerId", Name = "IX_tb_Order_CustomerID")]
[Index("VoucherId", Name = "IX_tb_Order_VoucherId")]
public partial class TbOrder
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal TotalAmount { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public Guid? UpdateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [Column("CustomerID")]
    public Guid? CustomerId { get; set; }

    [Column("AccountID")]
    public Guid? AccountId { get; set; }

    public int PaymentMethod { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? AmountShip { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? TotalAmountDiscount { get; set; }

    [StringLength(50)]
    public string? OrderCode { get; set; }

    [Column("OrderCodeGHN")]
    [StringLength(50)]
    [Unicode(false)]
    public string? OrderCodeGhn { get; set; }

    public Guid? AddressDeliveryId { get; set; }

    public bool? OrderCounter { get; set; }

    public string? ReasionCancel { get; set; }

    [StringLength(50)]
    public string? PhoneNumberCustomer { get; set; }

    public string? AddressCustomer { get; set; }

    public short PaymentStatus { get; set; }

    public bool IsCustomerTakeYourself { get; set; }

    public bool IsShippingAddressSameAsCustomerAddress { get; set; }

    public bool IsDraft { get; set; }

    public Guid? VoucherId { get; set; }

    [ForeignKey("AddressDeliveryId")]
    [InverseProperty("TbOrders")]
    public virtual TbAddressDelivery? AddressDelivery { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("TbOrders")]
    public virtual TbCustomer? Customer { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<TbOrderDetail> TbOrderDetails { get; set; } = new List<TbOrderDetail>();

    [ForeignKey("VoucherId")]
    [InverseProperty("TbOrders")]
    public virtual TbVoucher? Voucher { get; set; }
}
