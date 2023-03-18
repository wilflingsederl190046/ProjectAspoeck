﻿using Microsoft.EntityFrameworkCore;

namespace BreakfastDBLib;
public partial class BreakfastDBContext : DbContext
{
  public BreakfastDBContext() { }

  public BreakfastDBContext(DbContextOptions<BreakfastDBContext> options) : base(options) { }

  public virtual DbSet<Item> Items { get; set; }

  public virtual DbSet<Order> Orders { get; set; }

  public virtual DbSet<OrderItem> OrderItems { get; set; }

  public virtual DbSet<OrderState> OrderStates { get; set; }

  public virtual DbSet<Setting> Settings { get; set; }

  public virtual DbSet<User> Users { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
   => optionsBuilder.UseSqlServer("Server=(LocalDB)\\mssqllocaldb;attachdbfilename=C:\\Users\\lukas\\OneDrive\\Desktop\\POS\\#sonstigeProjekte\\CreateTestDataAspoeck\\BreakfastDBLib\\BreakfastDB.mdf;integrated security=True; MultipleActiveResultSets=True");
  //=> optionsBuilder.UseSqlServer("Server=(LocalDB)\\mssqllocaldb;attachdbfilename=C:\\4_Klasse\\PRASPÖCK\\Project\\Project\\ProjectAspoeck\\BreakfastDBLib\\BreakfastDB.mdf;integrated security=True; MultipleActiveResultSets=True");
  //

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Item>(entity =>
    {
      entity.HasKey(e => e.ItemId).HasName("PK__Items__727E838B0161AC90");

      entity.Property(e => e.ItemId).ValueGeneratedNever();
      entity.Property(e => e.Active)
              .IsRequired()
              .HasDefaultValueSql("((1))");
      entity.Property(e => e.Name)
              .HasMaxLength(255)
              .IsUnicode(false);
    });

    modelBuilder.Entity<Order>(entity =>
    {
      entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF73F8467C");

      entity.Property(e => e.OrderId).ValueGeneratedNever();
      entity.Property(e => e.OrderDate)
              .HasDefaultValueSql("(getdate())")
              .HasColumnType("datetime");

      entity.HasOne(d => d.OrderState).WithMany(p => p.Orders)
              .HasForeignKey(d => d.OrderStateId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_Orders_OrderState");

      entity.HasOne(d => d.User).WithMany(p => p.Orders)
              .HasForeignKey(d => d.UserId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_Orders_Users");
    });

    modelBuilder.Entity<OrderItem>(entity =>
    {
      entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED0681D0023EB4");

      entity.Property(e => e.OrderItemId).ValueGeneratedNever();

      entity.HasOne(d => d.Item).WithMany(p => p.OrderItems)
              .HasForeignKey(d => d.ItemId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_OrderItems_Items");

      entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
              .HasForeignKey(d => d.OrderId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_OrderItems_Orders");
    });

    modelBuilder.Entity<OrderState>(entity =>
    {
      entity.HasKey(e => e.OrderStateId).HasName("PK__OrderSta__5E701EFD52386C9B");

      entity.ToTable("OrderState");

      entity.Property(e => e.OrderStateId).ValueGeneratedNever();
      entity.Property(e => e.Name)
              .HasMaxLength(50)
              .IsUnicode(false);
    });

    modelBuilder.Entity<Setting>(entity =>
    {
      entity.HasKey(e => e.SettingId).HasName("PK__Settings__54372B1D8128CBCE");

      entity.Property(e => e.SettingId).ValueGeneratedNever();

      entity.HasOne(d => d.User).WithMany(p => p.Settings)
              .HasForeignKey(d => d.UserId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_Settings_Users");
    });

    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C2BE0530C");

      entity.Property(e => e.UserId).ValueGeneratedNever();
      entity.Property(e => e.Active)
              .IsRequired()
              .HasDefaultValueSql("((1))");
      entity.Property(e => e.ChipNumber)
              .HasMaxLength(50)
              .IsUnicode(false);
      entity.Property(e => e.CreatedDate)
              .HasDefaultValueSql("(getdate())")
              .HasColumnType("datetime");
      entity.Property(e => e.Email)
              .HasMaxLength(255)
              .IsUnicode(false);
      entity.Property(e => e.FirstName)
              .HasMaxLength(100)
              .IsUnicode(false);
      entity.Property(e => e.LastName)
              .HasMaxLength(100)
              .IsUnicode(false);
      entity.Property(e => e.UserName)
              .HasMaxLength(255)
              .IsUnicode(false);
      entity.Property(e => e.UserPassword)
              .HasMaxLength(255)
              .IsFixedLength();
    });

    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
