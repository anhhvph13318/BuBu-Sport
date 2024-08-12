using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Keyless]
[Table("tb_OrderDetail")]
public partial class TbOrderDetail
{
    [Column("ID")]
    public Guid Id { get; set; }

    [Column("ProductID")]
    public Guid ProductId { get; set; }

    [Column("OrderID")]
    public Guid OrderId { get; set; }

    public int Quantity { get; set; }
}
