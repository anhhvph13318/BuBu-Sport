using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbImage
{
    public Guid Id { get; set; }

    public string Url { get; set; } = null!;

    public string Type { get; set; } = null!;
    public bool? Status { get; set; }
    public bool? InAcitve { get; set; }

    public Guid? ProductId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public virtual ICollection<TbProduct> TbProducts { get; set; } = new List<TbProduct>();
    public virtual ICollection<TbProductDetail> ProductDetails { get; set; }
}
