using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbOrderDetail
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid OrderId { get; set; }

    public int Quantity { get; set; }

    public virtual TbOrder Order { get; set; } = null!;

    public virtual TbProduct Product { get; set; } = null!;
}
