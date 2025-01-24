using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbAddressDelivery
{
    public Guid Id { get; set; }

    public string ProvinceName { get; set; } = null!;

    public string? DistrictName { get; set; }

    public string? WardName { get; set; }

    public bool? Status { get; set; }

    public bool? IsDelete { get; set; }

    public Guid? AccountId { get; set; }

    public string? ReceiverName { get; set; }

    public string? ReceiverPhone { get; set; }

    public virtual TbAccount? Account { get; set; }

    public virtual ICollection<TbOrder> TbOrders { get; set; } = new List<TbOrder>();
}
