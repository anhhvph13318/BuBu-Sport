using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Voucher")]
public partial class TbVoucher
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string Code { get; set; } = null!;

    public int Discount { get; set; }

    public string? Description { get; set; }

    public int? Quantity { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    public short? Type { get; set; }

    [StringLength(50)]
    public string? Unit { get; set; }

    public short? Status { get; set; }

    public Guid? UpdateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public Guid CreateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("ProductID")]
    public Guid? ProductId { get; set; }

    [Column("CategoryID")]
    public Guid? CategoryId { get; set; }

    [Column("GroupCustomerID")]
    public Guid? GroupCustomerId { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? MaxDiscount { get; set; }

    [InverseProperty("Voucher")]
    public virtual ICollection<TbOrder> TbOrders { get; set; } = new List<TbOrder>();
}
