using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_OrderDetail")]
[Index("OrderId", Name = "IX_tb_OrderDetail_OrderID")]
[Index("ProductId", Name = "IX_tb_OrderDetail_ProductID")]
public partial class TbOrderDetail
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [Column("ProductID")]
    public Guid ProductId { get; set; }

    [Column("OrderID")]
    public Guid OrderId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("TbOrderDetails")]
    public virtual TbOrder Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("TbOrderDetails")]
    public virtual TbProduct Product { get; set; } = null!;
}
