using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Entity;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
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

    public virtual DbSet<TbCustomerVoucher> TbCustomerVouchers { get; set; }

    public virtual DbSet<TbFuntion> TbFuntions { get; set; }

    public virtual DbSet<TbFuntionForPermission> TbFuntionForPermissions { get; set; }

    public virtual DbSet<TbGroupCustomer> TbGroupCustomers { get; set; }

    public virtual DbSet<TbImage> TbImages { get; set; }

    public virtual DbSet<TbInvoice> TbInvoices { get; set; }

    public virtual DbSet<TbInvoiceDetail> TbInvoiceDetails { get; set; }

    public virtual DbSet<TbMaterial> TbMaterials { get; set; }

    public virtual DbSet<TbOrder> TbOrders { get; set; }

    public virtual DbSet<TbOrderDetail> TbOrderDetails { get; set; }

    public virtual DbSet<TbPaymentMethod> TbPaymentMethods { get; set; }

    public virtual DbSet<TbPermission> TbPermissions { get; set; }

    public virtual DbSet<TbPolicy> TbPolicies { get; set; }

    public virtual DbSet<TbProduct> TbProducts { get; set; }

    public virtual DbSet<TbPromotion> TbPromotions { get; set; }

    public virtual DbSet<TbProperty> TbProperties { get; set; }

    public virtual DbSet<TbSupplier> TbSuppliers { get; set; }

    public virtual DbSet<TbUser> TbUsers { get; set; }

    public virtual DbSet<TbUserFuntion> TbUserFuntions { get; set; }

    public virtual DbSet<TbUserGroup> TbUserGroups { get; set; }

    public virtual DbSet<TbVoucher> TbVouchers { get; set; }

    public virtual DbSet<TbWallet> TbWallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-I8QEIM8;Initial Catalog=DATABASE_ACV_13_01_2024;Integrated Security=True;Trust Server Certificate=True; Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<TbAccount>(entity =>
        {
            entity.ToTable("tb_Account");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AccountCode).HasMaxLength(50);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.Password).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.TbAccounts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_Account_tb_Customer");
        });

        modelBuilder.Entity<TbAddressDelivery>(entity =>
        {
            entity.ToTable("tb_AddressDelivery");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.DistrictId).HasColumnName("districtId");
            entity.Property(e => e.DistrictName).HasColumnName("districtName");
            entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            entity.Property(e => e.ProviceId).HasColumnName("proviceId");
            entity.Property(e => e.ProvinceName).HasColumnName("provinceName");
            entity.Property(e => e.ReceiverName)
                .HasMaxLength(255)
                .HasColumnName("receiverName");
            entity.Property(e => e.ReceiverPhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("receiverPhone");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.WardId).HasColumnName("wardId");
            entity.Property(e => e.WardName).HasColumnName("wardName");

            entity.HasOne(d => d.Account).WithMany(p => p.TbAddressDeliveries)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_AddressDelivery_tb_Account");
        });

        modelBuilder.Entity<TbCart>(entity =>
        {
            entity.ToTable("tb_Cart");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbCartDetail>(entity =>
        {
            entity.ToTable("tb_CartDetail");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Cart).WithMany(p => p.TbCartDetails)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK_tb_CartDetail_tb_Cart");

            entity.HasOne(d => d.Product).WithMany(p => p.TbCartDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_tb_CartDetail_tb_Produst");
        });

        modelBuilder.Entity<TbCategory>(entity =>
        {
            entity.ToTable("tb_Category");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDelete).HasDefaultValueSql("((1))");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbColor>(entity =>
        {
            entity.ToTable("tb_Color");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<TbCustomer>(entity =>
        {
            entity.ToTable("tb_Customer");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GroupCustomerId).HasColumnName("GroupCustomerID");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Status).HasMaxLength(250);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.YearOfBirth).HasColumnType("datetime");

            entity.HasOne(d => d.GroupCustomer).WithMany(p => p.TbCustomers)
                .HasForeignKey(d => d.GroupCustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_Customer_tb_GroupCustomer");
        });

        modelBuilder.Entity<TbCustomerVoucher>(entity =>
        {
            entity.HasKey(e => e.CustomerId);

            entity.ToTable("tb_CustomerVoucher");

            entity.Property(e => e.CustomerId)
                .ValueGeneratedNever()
                .HasColumnName("CustomerID");
            entity.Property(e => e.VoucherCode).HasMaxLength(50);
            entity.Property(e => e.VoucherId).HasColumnName("VoucherID");

            entity.HasOne(d => d.Customer).WithOne(p => p.TbCustomerVoucher)
                .HasForeignKey<TbCustomerVoucher>(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_CustomerVoucher_tb_Customer");

            entity.HasOne(d => d.Voucher).WithMany(p => p.TbCustomerVouchers)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_CustomerVoucher_tb_Voucher");
        });

        modelBuilder.Entity<TbFuntion>(entity =>
        {
            entity.ToTable("tb_Funtions");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Code).HasMaxLength(50);
        });

        modelBuilder.Entity<TbFuntionForPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_FuntionForPermission");

            entity.ToTable("tb_FuntionForPermission");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.FuntionId).HasColumnName("FuntionID");
            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");

            entity.HasOne(d => d.Funtion).WithMany(p => p.TbFuntionForPermissions)
                .HasForeignKey(d => d.FuntionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_FuntionForPermission_tb_Funtions");

            entity.HasOne(d => d.Permission).WithMany(p => p.TbFuntionForPermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_FuntionForPermission_tb_Permission");
        });

        modelBuilder.Entity<TbGroupCustomer>(entity =>
        {
            entity.ToTable("tb_GroupCustomer");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.IsDelete)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Name).HasMaxLength(250);
        });

        modelBuilder.Entity<TbImage>(entity =>
        {
            entity.ToTable("tb_Image");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.InAcitve)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Type).HasMaxLength(250);
        });

        modelBuilder.Entity<TbInvoice>(entity =>
        {
            entity.ToTable("tb_Invoice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InputDate).HasColumnType("datetime");
            entity.Property(e => e.IsDelete)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.Unit).HasMaxLength(250);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Supplier).WithMany(p => p.TbInvoices)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_Invoice_tb_Supplier");
        });

        modelBuilder.Entity<TbInvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.InvoiceId);

            entity.ToTable("tb_InvoiceDetail");

            entity.Property(e => e.InvoiceId)
                .ValueGeneratedNever()
                .HasColumnName("InvoiceID");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Unit).HasMaxLength(50);

            entity.HasOne(d => d.Invoice).WithOne(p => p.TbInvoiceDetail)
                .HasForeignKey<TbInvoiceDetail>(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_InvoiceDetail_tb_Invoice");

            entity.HasOne(d => d.Product).WithMany(p => p.TbInvoiceDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_InvoiceDetail_tb_Produst");
        });

        modelBuilder.Entity<TbMaterial>(entity =>
        {
            entity.ToTable("tb_Material");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<TbOrder>(entity =>
        {
            entity.ToTable("tb_Order");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.AmountShip).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.OrderCode).HasMaxLength(50);
            entity.Property(e => e.OrderCodeGhn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OrderCodeGHN");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.PhoneNumberCustomer).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TotalAmountDiscount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbOrderDetail>(entity =>
        {
            entity.ToTable("tb_OrderDetail");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Order).WithMany(p => p.TbOrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_OrderDetail_tb_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.TbOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_OrderDetail_tb_Produst");
        });

        modelBuilder.Entity<TbPaymentMethod>(entity =>
        {
            entity.ToTable("tb_PaymentMethod");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CardNumber).HasMaxLength(50);
            entity.Property(e => e.InActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<TbPermission>(entity =>
        {
            entity.ToTable("tb_Permission");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDelete).HasDefaultValueSql("((1))");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbPolicy>(entity =>
        {
            entity.ToTable("tb_Policy");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tb_Produst");

            entity.ToTable("tb_Products");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.IsDelete)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Length).HasColumnName("length");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PriceNet).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.Vat).HasColumnName("VAT");
            entity.Property(e => e.Weight).HasColumnName("weight");
            entity.Property(e => e.Width).HasColumnName("width");

            entity.HasOne(d => d.Category).WithMany(p => p.TbProducts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_Produst_tb_Category");
        });

        modelBuilder.Entity<TbPromotion>(entity =>
        {
            entity.ToTable("tb_Promotion");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.PolicyId).HasColumnName("PolicyID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Policy).WithMany(p => p.TbPromotions)
                .HasForeignKey(d => d.PolicyId)
                .HasConstraintName("FK_tb_Promotion_tb_Policy");
        });

        modelBuilder.Entity<TbProperty>(entity =>
        {
            entity.ToTable("tb_Properties");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbSupplier>(entity =>
        {
            entity.ToTable("tb_Supplier");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.IsDelete)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.ProvideProducst).HasMaxLength(250);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbUser>(entity =>
        {
            entity.ToTable("tb_User");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.InActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Password).HasMaxLength(250);
            entity.Property(e => e.Position).HasMaxLength(250);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UserCode).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(250);

            entity.HasOne(d => d.UserGroup).WithMany(p => p.TbUsers)
                .HasForeignKey(d => d.UserGroupId)
                .HasConstraintName("FK_tb_User_tb_UserGroup");
        });

        modelBuilder.Entity<TbUserFuntion>(entity =>
        {
            entity.ToTable("tb_UserFuntion");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.FuntionForPermissionId).HasColumnName("FuntionForPermissionID");
            entity.Property(e => e.GroupUserId).HasColumnName("GroupUserID");

            entity.HasOne(d => d.FuntionForPermission).WithMany(p => p.TbUserFuntions)
                .HasForeignKey(d => d.FuntionForPermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_UserFuntion_tb_FuntionForPermission");
        });

        modelBuilder.Entity<TbUserGroup>(entity =>
        {
            entity.ToTable("tb_UserGroup");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbVoucher>(entity =>
        {
            entity.ToTable("tb_Voucher");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.GroupCustomerId).HasColumnName("GroupCustomerID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbWallet>(entity =>
        {
            entity.ToTable("tb_Wallet");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Surplus).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Account).WithMany(p => p.TbWallets)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tb_Wallet_tb_Account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
