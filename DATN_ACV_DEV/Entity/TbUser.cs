using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbUser
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Position { get; set; }

    public string? UserCode { get; set; }

    public string? FullName { get; set; }

    public bool? InActive { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid CreateBy { get; set; }

    public Guid? UserGroupId { get; set; }

    public virtual TbUserGroup? UserGroup { get; set; }
}
