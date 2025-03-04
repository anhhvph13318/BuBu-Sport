using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Category")]
public partial class TbCategory
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [StringLength(250)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Status { get; set; }

    public bool? IsDelete { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public Guid CreateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public Guid? ImageId { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<TbProduct> TbProducts { get; set; } = new List<TbProduct>();
}
