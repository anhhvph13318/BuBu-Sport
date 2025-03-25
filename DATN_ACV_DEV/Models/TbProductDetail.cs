using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_ProductDetail")]
public partial class TbProductDetail
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    [Column("ImageID")]
    public Guid? ImageId { get; set; }

    [Column("ColorID")]
    public Guid? ColorId { get; set; }

    [Column("SizeID")]
    public Guid? SizeId { get; set; }

    [Column("ProductID")]
    public Guid? ProductId { get; set; }

    [ForeignKey("ImageId")]
    [InverseProperty("TbProductDetails")]
    public virtual TbImage? Image { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("TbProductDetails")]
    public virtual TbProduct? Product { get; set; }

    [ForeignKey("SizeId")]
    [InverseProperty("TbProductDetails")]
    public virtual TbSize? Size { get; set; }
}
