using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Properties")]
public partial class TbProperty
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    [Column("CategoryID")]
    public Guid? CategoryId { get; set; }

    public Guid CreateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public Guid? ProductId { get; set; }

    public bool? Active { get; set; }
}
