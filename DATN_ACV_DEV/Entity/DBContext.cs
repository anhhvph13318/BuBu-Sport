using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Entity;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)///
        : base(options)
    {
    }

    public virtual DbSet<TbAccount> TbAccounts { get; set; }

    public virtual DbSet<TbAddressDelivery> TbAddressDeliveries { get; set; }

    public virtual DbSet<TbCart> TbCarts { get; set; }

    public virtual DbSet<TbCartDetail> TbCartDetails { get; set; }

    public virtual DbSet<TbCategory> TbCategories { get; set; }

    public virtual DbSet<TbColor> TbColors { get; set; }

    public virtual DbSet<TbCustomer> TbCustomers { get; set; }



    public virtual DbSet<TbImage> TbImages { get; set; }

    public virtual DbSet<TbMaterial> TbMaterials { get; set; }

    public virtual DbSet<TbOrder> TbOrders { get; set; }

    public virtual DbSet<TbOrderDetail> TbOrderDetails { get; set; }



    public virtual DbSet<TbProduct> TbProducts { get; set; }


    public virtual DbSet<TbProperty> TbProperties { get; set; }



    public virtual DbSet<TbUser> TbUsers { get; set; }



    public virtual DbSet<TbVoucher> TbVouchers { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=TW4NENH\\TUANANH;Initial Catalog=DB_SmartHouse_12_08;Integrated Security=True;Trust Server Certificate=True; Encrypt=False;"); }

    
