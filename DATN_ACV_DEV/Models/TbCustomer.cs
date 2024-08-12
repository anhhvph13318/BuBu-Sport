using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Customer")]
public partial class TbCustomer
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [StringLength(250)]
    public string? Name { get; set; }

    public string? Adress { get; set; }

    public int? Rank { get; set; }

    [StringLength(250)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? YearOfBirth { get; set; }

    public int? Sex { get; set; }

    public int? Point { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    [Column("GroupCustomerID")]
    public Guid GroupCustomerId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Phone { get; set; }
}
