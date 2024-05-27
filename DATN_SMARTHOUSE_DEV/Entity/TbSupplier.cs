using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbSupplier
{
    public Guid Id { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ProvideProducst { get; set; }

    public string? Name { get; set; }

    public string? Adress { get; set; }

    public bool? IsDelete { get; set; }

    public bool? InActive { get; set; }

    public Guid? UpdateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<TbInvoice> TbInvoices { get; set; } = new List<TbInvoice>();
}
