using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbCustomer
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Adress { get; set; }

    public string? Phone { get; set; }

    public int? Rank { get; set; }

    public string? Status { get; set; }

    public DateTime? YearOfBirth { get; set; }

    public int? Sex { get; set; }

    public int? Point { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public Guid GroupCustomerId { get; set; }

    public virtual ICollection<TbAccount> TbAccounts { get; set; } = new List<TbAccount>();

    public virtual ICollection<TbOrder> TbOrders { get; set; } = new List<TbOrder>();
}
