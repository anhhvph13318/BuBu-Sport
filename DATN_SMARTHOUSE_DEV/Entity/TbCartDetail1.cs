using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATN_ACV_DEV.Entity;

public partial class TbCartDetail
{
    [NotMapped] 
    public TbProduct tbProduct { get; set; }
    [NotMapped]
    public TbImage? tbImage { get; set; }
}
