using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATN_ACV_DEV.Entity;

public partial class TbOrder
{
    [NotMapped]
    public TbCustomer customer { get; set; }
    [NotMapped]
    public TbPaymentMethod paymentMethod { get; set; }
    [NotMapped]
    public List<TbOrderDetail> orderDetail { get; set; }
}
