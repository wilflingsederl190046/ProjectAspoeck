using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace BreakfastDbLib;

public partial class BreakfastDBContext : DbContext
{
  public BreakfastDBContext()
  {
  }

  public BreakfastDBContext(DbContextOptions<BreakfastDBContext> options)
      : base(options)
  {
  }

  public virtual DbSet<Image> Images { get; set; }

  public virtual DbSet<Item> Items { get; set; }

  public virtual DbSet<Order> Orders { get; set; }

  public virtual DbSet<OrderItem> OrderItems { get; set; }

  public virtual DbSet<OrderState> OrderStates { get; set; }

  public virtual DbSet<Setting> Settings { get; set; }

  public virtual DbSet<User> Users { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  //Lukas
 => optionsBuilder.UseSqlServer(@"Server=(LocalDB)\mssqllocaldb;attachdbfilename=C:\Users\lukas\OneDrive\Desktop\POS\#sonstigeProjekte\ProjectAspoeck\BreakfastDbLib\BreakfastDb.mdf;integrated security=False;MultipleActiveResultSets=True");

  //Ben
  // => optionsBuilder.UseSqlServer(@"Server=(LocalDB)\mssqllocaldb;attachdbfilename=C:\Temp\BreakfastDb.mdf;integrated security=True;MultipleActiveResultSets=True");

  //Simon
  //=> optionsBuilder.UseSqlServer(@"Server=(LocalDB)\mssqllocaldb;attachdbfilename=C:\Temp\BreakfastDb.mdf;integrated security=True;MultipleActiveResultSets=True");


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Item>(entity =>
    {
      entity.HasIndex(e => e.ImageId, "IX_Items_ImageId");

      entity.HasOne(d => d.Image).WithMany(p => p.Items).HasForeignKey(d => d.ImageId);
    });

    modelBuilder.Entity<Order>(entity =>
    {
      entity.HasIndex(e => e.OrderStateId, "IX_Orders_OrderStateId");

      entity.HasIndex(e => e.UserId, "IX_Orders_UserId");

      entity.HasOne(d => d.OrderState).WithMany(p => p.Orders).HasForeignKey(d => d.OrderStateId);

      entity.HasOne(d => d.User).WithMany(p => p.Orders).HasForeignKey(d => d.UserId);
    });

    modelBuilder.Entity<OrderItem>(entity =>
    {
      entity.HasIndex(e => e.ItemId, "IX_OrderItems_ItemId");

      entity.HasIndex(e => e.OrderId, "IX_OrderItems_OrderId");

      entity.HasOne(d => d.Item).WithMany(p => p.OrderItems).HasForeignKey(d => d.ItemId);

      entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasForeignKey(d => d.OrderId);
    });

    modelBuilder.Entity<Setting>(entity =>
    {
      entity.HasIndex(e => e.UserId, "IX_Settings_UserId");

      entity.HasOne(d => d.User).WithMany(p => p.Settings).HasForeignKey(d => d.UserId);
    });

    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
