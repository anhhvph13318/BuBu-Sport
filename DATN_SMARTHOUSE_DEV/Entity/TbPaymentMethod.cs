using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbPaymentMethod
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? CardNumber { get; set; }

    public string? Type { get; set; }

    public bool? InActive { get; set; }
}
