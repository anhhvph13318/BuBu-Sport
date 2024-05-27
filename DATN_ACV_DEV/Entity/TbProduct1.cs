using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATN_ACV_DEV.Entity;

public partial class TbProduct
{
    [NotMapped]
    public TbImage tb_Image { get; set; }
}
