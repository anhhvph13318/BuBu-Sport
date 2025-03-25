using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_User")]
public partial class TbUser
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [StringLength(250)]
    public string UserName { get; set; } = null!;

    [StringLength(250)]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? Password { get; set; }

    [StringLength(250)]
    public string? Position { get; set; }

    [StringLength(20)]
    public string? UserCode { get; set; }

    [StringLength(250)]
    public string? FullName { get; set; }

    [Required]
    public bool? InActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public Guid CreateBy { get; set; }

    public Guid? UserGroupId { get; set; }
}
