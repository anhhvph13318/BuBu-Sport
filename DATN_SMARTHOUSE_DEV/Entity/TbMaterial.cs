using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbMaterial
{
    public Guid Id { get; set; }

    public string Material { get; set; } = null!;

    public string Status { get; set; } = null!;
}
