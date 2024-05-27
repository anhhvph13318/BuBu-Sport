using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbCartDetail
{
    public Guid Id { get; set; }

    public int? Quantity { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? CartId { get; set; }

    public virtual TbCart? Cart { get; set; }

    public virtual TbProduct? Product { get; set; }
}
