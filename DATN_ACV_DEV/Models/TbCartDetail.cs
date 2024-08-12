using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_CartDetail")]
public partial class TbCartDetail
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    public int? Quantity { get; set; }

    [Column("ProductID")]
    public Guid? ProductId { get; set; }

    [Column("CartID")]
    public Guid? CartId { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("TbCartDetails")]
    public virtual TbCart? Cart { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("TbCartDetails")]
    public virtual TbProduct? Product { get; set; }
}
