using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Image")]
public partial class TbImage
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    public string Url { get; set; } = null!;

    [StringLength(250)]
    public string Type { get; set; } = null!;

    [Required]
    public bool? InAcitve { get; set; }

    [Column("ProductID")]
    public Guid? ProductId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }
}
