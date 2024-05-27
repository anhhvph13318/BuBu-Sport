using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbProperty
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? CategoryId { get; set; }

    public Guid CreateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? ProductId { get; set; }

    public bool? Active { get; set; }
}
