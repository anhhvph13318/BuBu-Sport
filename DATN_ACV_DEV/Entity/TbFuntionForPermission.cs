using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbFuntionForPermission
{
    public Guid Id { get; set; }

    public Guid PermissionId { get; set; }

    public Guid FuntionId { get; set; }

    public virtual TbFuntion Funtion { get; set; } = null!;

    public virtual TbPermission Permission { get; set; } = null!;

    public virtual ICollection<TbUserFuntion> TbUserFuntions { get; set; } = new List<TbUserFuntion>();
}
