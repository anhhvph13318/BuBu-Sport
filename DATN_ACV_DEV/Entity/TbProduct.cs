﻿using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbProduct
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int? Status { get; set; }

    public string? Description { get; set; }

    public decimal? PriceNet { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? ImageId { get; set; }

    public Guid CategoryId { get; set; }

    public bool? Vat { get; set; }

    public string? Warranty { get; set; }

    public string? Color { get; set; }

    public string? Material { get; set; }

    public string? Code { get; set; }

    public int? Size { get; set; }
    public string Brand { get; set; }
    public virtual TbCategory Category { get; set; } = null!;

    public virtual TbImage? Image { get; set; }

    public virtual ICollection<TbCartDetail> TbCartDetails { get; set; } = new List<TbCartDetail>();

    public virtual ICollection<TbOrderDetail> TbOrderDetails { get; set; } = new List<TbOrderDetail>();
    public virtual ICollection<TbProductDetail> ProductDetails { get; set; }
}
