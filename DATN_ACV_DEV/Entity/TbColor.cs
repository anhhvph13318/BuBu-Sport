using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbColor
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Status { get; set; }
}
