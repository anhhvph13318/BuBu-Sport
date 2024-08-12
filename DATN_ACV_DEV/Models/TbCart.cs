using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Cart")]
public partial class TbCart
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    [Column("AccountID")]
    public Guid? AccountId { get; set; }

    public bool? CartAdmin { get; set; }

    public Guid? CreateBy { get; set; }

    [InverseProperty("Cart")]
    public virtual ICollection<TbCartDetail> TbCartDetails { get; set; } = new List<TbCartDetail>();
}
