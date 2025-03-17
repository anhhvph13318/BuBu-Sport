using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbSize
{
    public Guid Id { get; set; }

    public string? SizeName { get; set; }

    public double? FootLength { get; set; }

    public int? Quantity { get; set; }

    public virtual ICollection<TbProductDetail> TbProductDetails { get; set; } = new List<TbProductDetail>();
}
