using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbAddressDelivery
{
    public Guid Id { get; set; }

    public string ProvinceName { get; set; } = string.Empty;

    public int ProviceId { get; set; }

    public string DistrictName { get; set; } = string.Empty;

    public int DistrictId { get; set; }

    public string WardName { get; set; } = string.Empty;
    public string WardCode { get; set; } = string.Empty;

    public int WardId { get; set; }

    public bool? Status { get; set; }

    public bool? IsDelete { get; set; }

    public Guid? AccountId { get; set; }

    public string ReceiverName { get; set; } = string.Empty;

    public string ReceiverPhone { get; set; } = string.Empty;

    public virtual TbAccount? Account { get; set; } = null!;
}
