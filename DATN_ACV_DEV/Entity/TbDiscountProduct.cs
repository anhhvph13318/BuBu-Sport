using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbDiscountProduct
{
    public Guid Id { get; set; }

    public Guid DiscountId { get; set; }

    public Guid ProductId { get; set; }

    public virtual TbDiscount Discount { get; set; } = null!;

    public virtual TbProduct Product { get; set; } = null!;
}
