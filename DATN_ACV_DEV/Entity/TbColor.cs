using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATN_ACV_DEV.Entity;

public partial class TbColor
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public virtual ICollection<TbProductDetail> ProductDetails { get; set; } = new List<TbProductDetail>();

}
