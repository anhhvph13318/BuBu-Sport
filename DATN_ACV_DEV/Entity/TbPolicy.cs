using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbPolicy
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? Type { get; set; }

    public string? Description { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public Guid? ImageId { get; set; }

    public virtual ICollection<TbPromotion> TbPromotions { get; set; } = new List<TbPromotion>();
}
