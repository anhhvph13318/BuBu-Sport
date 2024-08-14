using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_PropertiesDetail")]
public partial class TbPropertiesDetail
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [Column("PropertiesID")]
    public Guid? PropertiesId { get; set; }

    [Column("ProductID")]
    public Guid? ProductId { get; set; }
}
