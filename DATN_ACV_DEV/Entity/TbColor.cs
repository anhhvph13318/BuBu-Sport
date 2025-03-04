using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbColor
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }
}
