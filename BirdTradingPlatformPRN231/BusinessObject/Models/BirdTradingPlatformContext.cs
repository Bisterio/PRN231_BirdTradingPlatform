﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace BusinessObject.Models
{
    public partial class BirdTradingPlatformContext : DbContext
    {
        public BirdTradingPlatformContext()
        {
        }

        public BirdTradingPlatformContext(DbContextOptions<BirdTradingPlatformContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Store> Stores { get; set; } = null!;
        public virtual DbSet<UserAccount> UserAccounts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(GetConnectionString());
            }
        }

        private string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var strConn = config["ConnectionStrings:BirdTradingPlatform"];
            return strConn;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.CreatedAt)
                    .HasPrecision(6)
                    .HasColumnName("created_at");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(255)
                    .HasColumnName("payment_method");

                entity.Property(e => e.Phone)
                    .HasMaxLength(25)
                    .HasColumnName("phone");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_amount");

                entity.Property(e => e.TotalAmountPreShipping)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_amount_pre_shipping");

                entity.Property(e => e.TotalItem).HasColumnName("total_item");

                entity.Property(e => e.TotalShippingCost)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_shipping_cost");

                entity.Property(e => e.UpdatedAt)
                    .HasPrecision(6)
                    .HasColumnName("updated_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK90lxxrxlt4chf273vcm9pi8ak");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FKn46c37756vcjh8mq42bv4dbfp");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId })
                    .HasName("PK__order_it__022945F608312800");

                entity.ToTable("order_item");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKs234mi6jususbx4b37k44cipy");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK551losx9j75ss5d6bfsqvijna");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CreatedAt)
                    .HasPrecision(6)
                    .HasColumnName("created_at");

                entity.Property(e => e.Description)
                    .HasMaxLength(2500)
                    .HasColumnName("description");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Stock).HasColumnName("stock");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("unit_price");

                entity.Property(e => e.UpdatedAt)
                    .HasPrecision(6)
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK1mtsbur82frn64de7balymq9s");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FKjlfidudl1gwqem0flrlomvlcl");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("store");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.CreatedAt)
                    .HasPrecision(6)
                    .HasColumnName("created_at");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedAt)
                    .HasPrecision(6)
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__user_acc__B9BE370FBFFA4ADA");

                entity.ToTable("user_account");

                entity.HasIndex(e => e.Email, "UQ__user_acc__AB6E61648FE6B68A")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.CreatedAt)
                    .HasPrecision(6)
                    .HasColumnName("created_at");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.EmailVerified).HasColumnName("email_verified");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(25)
                    .HasColumnName("phone");

                entity.Property(e => e.Role)
                    .HasMaxLength(25)
                    .HasColumnName("role");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.UpdatedAt)
                    .HasPrecision(6)
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.UserAccounts)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK7vqhqxt45ua0j213qal23oqd5");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
