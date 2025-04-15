using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATN_ACV_DEV.Entity;

public partial class TbProductDetail
{
    public Guid Id { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public Guid? ImageId { get; set; }
    [ForeignKey("Color")]
    public Guid? ColorId { get; set; }

    public Guid? SizeId { get; set; }

    public Guid? ProductId { get; set; }
    public DateTime? CreateDate { get; set; }

    public virtual TbImage? Image { get; set; }

    public virtual TbProduct? Product { get; set; }
    public virtual TbSize? Size { get; set; }
    public virtual TbColor? Color { get; set; } = null!;

}
