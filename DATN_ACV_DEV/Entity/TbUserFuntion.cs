using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbUserFuntion
{
    public Guid Id { get; set; }

    public Guid GroupUserId { get; set; }

    public Guid FuntionForPermissionId { get; set; }

    public virtual TbFuntionForPermission FuntionForPermission { get; set; } = null!;
}
