using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbDiscount
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? DiscountType { get; set; }

    public decimal? DiscountValue { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<TbDiscountProduct> TbDiscountProducts { get; set; } = new List<TbDiscountProduct>();
}
