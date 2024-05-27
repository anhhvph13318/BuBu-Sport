using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbPermission
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public int? SortIndex { get; set; }

    public bool? IsDelete { get; set; }

    public Guid CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual ICollection<TbFuntionForPermission> TbFuntionForPermissions { get; set; } = new List<TbFuntionForPermission>();
}
