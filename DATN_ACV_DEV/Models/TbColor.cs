using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Color")]
public partial class TbColor
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Status { get; set; }
}
