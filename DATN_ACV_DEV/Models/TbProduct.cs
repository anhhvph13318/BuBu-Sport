using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Products")]
public partial class TbProduct
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [StringLength(250)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "decimal(18, 0)")]
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int? Status { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? PriceNet { get; set; }

    public Guid? UpdateBy { get; set; }

    [Required]
    public bool? IsDelete { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public Guid CreateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [Column("ImageID")]
    public Guid? ImageId { get; set; }

    [Column("CategoryID")]
    public Guid CategoryId { get; set; }

    [Column("VAT")]
    public bool? Vat { get; set; }

    public string? Warranty { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    public string? Material { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Code { get; set; }

    [Column("weight")]
    public int? Weight { get; set; }

    [Column("length")]
    public int? Length { get; set; }

    [Column("width")]
    public int? Width { get; set; }

    [Column("height")]
    public int? Height { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("TbProducts")]
    public virtual TbCategory Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<TbCartDetail> TbCartDetails { get; set; } = new List<TbCartDetail>();
}
