using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbPromotion
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int PrecentDiscount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? PolicyId { get; set; }

    public Guid CreateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual TbPolicy? Policy { get; set; }
}
