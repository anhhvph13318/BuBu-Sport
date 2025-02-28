﻿namespace DATN_ACV_DEV.Entity;

public partial class TbVoucher
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int Discount { get; set; }

    public string? Description { get; set; }

    public int? Quantity { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public VoucherType Type { get; set; }

   

    public VoucherUnit Unit { get; set; }
    public decimal MaxDiscount { get; set; }
    //public decimal RequiredTotalAmount { get; set; }

    public Status Status { get; set; } = Status.Valid;

    public Guid? UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid CreateBy { get; set; }

    public DateTime CreateDate { get; set; }
    public virtual ICollection<TbOrder> Orders { get; set; } = new List<TbOrder>();
}

public enum VoucherUnit
{
    Percent,
    Money
}

public enum VoucherType : short
{
    Voucher = 0
}

public enum Status : short
{
    Closed = 1,
    Valid = 2
}