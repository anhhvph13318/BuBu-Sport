using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbAccount
{
    public Guid Id { get; set; }

    public string? AccountCode { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? PhoneNumber { get; set; }

    public Guid CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid CustomerId { get; set; }

    public virtual TbCustomer Customer { get; set; } = null!;

    public virtual ICollection<TbAddressDelivery> TbAddressDeliveries { get; set; } = new List<TbAddressDelivery>();

    public virtual ICollection<TbWallet> TbWallets { get; set; } = new List<TbWallet>();
}
