using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbUserGroup
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsDelete { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<TbUser> TbUsers { get; set; } = new List<TbUser>();
}
