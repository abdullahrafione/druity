using OnlineShop.DomainEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace OnlineShop.DbFactory
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext() : base("constring")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 300;
        }

        public DbSet<Organisation> Organisation { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductStock> ProductStock { get; set; }
        public DbSet<Colour> Colour { get; set; }
        public DbSet<Size> Size { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<OrderStatusLogs> OrderStatusLogs { get; set; }
       // public DbSet<PaymentMethod> PaymentMethod { get; set; }
        public DbSet<PaymentStatus> PaymentStatus { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<GenderTag> GenderTag { get; set; }
        public DbSet<AccountHead> AccountHead { get; set; }
        public DbSet<Expense> Expense { get; set; }
        public DbSet<Income> Income { get; set; }
        public DbSet<Ledger> Ledger { get; set; }
        public DbSet<SaleInvoice> SaleInvoice { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Tell Code First to ignore PluralizingTableName convention
            // If you keep this convention then the generated tables will have pluralized names.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Disable the default cascade behaviour of entity framework
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<User>().HasRequired(x => x.Organisation).WithMany(x => x.Users).HasForeignKey(x => x.OrganisationId).WillCascadeOnDelete(true);
            modelBuilder.Entity<Product>().HasRequired(x => x.Organisation).WithMany(x => x.Products).HasForeignKey(x => x.OrganisationId).WillCascadeOnDelete(true);
            modelBuilder.Entity<Product>().HasRequired(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId).WillCascadeOnDelete(true);
            modelBuilder.Entity<ProductStock>().HasRequired(x => x.Product).WithMany(x => x.ProductStock).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ProductStock>().HasRequired(x => x.Colour).WithMany(x => x.ProductStock).HasForeignKey(x => x.ColourId).WillCascadeOnDelete(true);
            modelBuilder.Entity<ProductStock>().HasRequired(x => x.Size).WithMany(x => x.ProductStock).HasForeignKey(x => x.SizeID).WillCascadeOnDelete(true);
            modelBuilder.Entity<Image>().HasRequired(x => x.Product).WithMany(x => x.Images).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(true);
            modelBuilder.Entity<Order>().HasRequired(x => x.User).WithMany(x => x.Order).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
           // modelBuilder.Entity<Order>().HasRequired(x => x.Organisation).WithMany(x => x.Order).HasForeignKey(x => x.OrganisationId).WillCascadeOnDelete(true);
            modelBuilder.Entity<OrderDetail>().HasRequired(x => x.Order).WithMany(x => x.OrderDetails).HasForeignKey(x => x.OrderId).WillCascadeOnDelete(true);
            modelBuilder.Entity<OrderDetail>().HasRequired(x => x.ProductStock).WithMany(x => x.OrderDetails).HasForeignKey(x => x.ProductStockId).WillCascadeOnDelete(true);
            modelBuilder.Entity<OrderDetail>().HasRequired(x => x.OrderStatus).WithMany(x => x.OrderDetail).HasForeignKey(x => x.OrderStatusId).WillCascadeOnDelete(true);
            modelBuilder.Entity<OrderStatusLogs>().HasRequired(x => x.OrderDetail).WithMany(x => x.OrderStatusLogs).HasForeignKey(x => x.OrderDetailId).WillCascadeOnDelete(true);
            modelBuilder.Entity<Payment>().HasRequired(x => x.Order).WithMany(x => x.Payment).HasForeignKey(x => x.OrderId).WillCascadeOnDelete(true);
            modelBuilder.Entity<Payment>().HasRequired(x => x.PaymentStatus).WithMany(x => x.Payment).HasForeignKey(x => x.PaymentStatusId).WillCascadeOnDelete(true);
            modelBuilder.Entity<Product>().HasRequired(x => x.GenderTag).WithMany(x => x.Products).HasForeignKey(x => x.GenderTagId).WillCascadeOnDelete(true);
        }
    }
}