using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Models;

[Table("tb_Account")]
[Index("CustomerId", Name = "IX_tb_Account_CustomerID")]
public partial class TbAccount
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [StringLength(50)]
    public string? AccountCode { get; set; }

    [StringLength(250)]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? Password { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    public Guid CreateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("CustomerID")]
    public Guid? CustomerId { get; set; }

    public Guid? EmployeeId { get; set; }

    public int? Role { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("TbAccounts")]
    public virtual TbCustomer? Customer { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<TbAddressDelivery> TbAddressDeliveries { get; set; } = new List<TbAddressDelivery>();
}
