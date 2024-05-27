using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbWallet
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string Code { get; set; } = null!;

    public decimal? Surplus { get; set; }

    public Guid CreateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public string? Status { get; set; }

    public Guid AccountId { get; set; }

    public virtual TbAccount Account { get; set; } = null!;
}
