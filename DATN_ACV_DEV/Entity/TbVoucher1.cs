using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATN_ACV_DEV.Entity;

public partial class TbVoucher
{
    [NotMapped]
    public TbProduct product { get; set; }
    [NotMapped]
    public TbCategory category { get; set; }
    [NotMapped]
    public TbGroupCustomer groupCustomer { get; set; }

}
