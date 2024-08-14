using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_AddressDelivery")]
public partial class TbAddressDelivery
{
    [Key]
    public Guid Id { get; set; }

    [Column("provinceName")]
    public string ProvinceName { get; set; } = null!;

    [Column("districtName")]
    public string? DistrictName { get; set; }

    [Column("wardName")]
    public string? WardName { get; set; }

    [Column("status")]
    public bool? Status { get; set; }

    [Column("isDelete")]
    public bool? IsDelete { get; set; }

    [Column("accountId")]
    public Guid? AccountId { get; set; }

    [Column("receiverName")]
    [StringLength(255)]
    public string? ReceiverName { get; set; }

    [Column("receiverPhone")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ReceiverPhone { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("TbAddressDeliveries")]
    public virtual TbAccount? Account { get; set; }
}
