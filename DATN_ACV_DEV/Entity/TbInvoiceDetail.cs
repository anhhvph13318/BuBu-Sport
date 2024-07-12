using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbInvoiceDetail
{
    public Guid InvoiceId { get; set; }

    public Guid ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? Quantity { get; set; }

    public decimal Price { get; set; }

    public string? Unit { get; set; }

    public virtual TbInvoice Invoice { get; set; } = null!;

    public virtual TbProduct Product { get; set; } = null!;
}
