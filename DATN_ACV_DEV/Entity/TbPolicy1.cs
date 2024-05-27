using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATN_ACV_DEV.Entity;

public partial class TbPolicy
{
    [NotMapped]
    public TbImage Image { get; set; }
}
