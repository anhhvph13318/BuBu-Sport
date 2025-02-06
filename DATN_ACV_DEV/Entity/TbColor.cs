using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbColor
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Status { get; set; }

    public bool? IsDelete { get; set; }
    public DateTime CreateDate { get; set; }
    public Guid CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }
    public Guid UpdatesBy { get; set; }
    public Guid? ImageId { get; set; }

    public virtual ICollection<TbProductDetail> ProductDetails { get; set; }
}
