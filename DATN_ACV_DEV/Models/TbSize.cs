using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Size")]
public partial class TbSize
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [StringLength(50)]
    public string? SizeName { get; set; }

    public double? FootLength { get; set; }

    public int? Quantity { get; set; }

    [InverseProperty("Size")]
    public virtual ICollection<TbProductDetail> TbProductDetails { get; set; } = new List<TbProductDetail>();
}
