namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finances1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountHead",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Expense",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountHeadId = c.Int(nullable: false),
                        Description = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountHead", t => t.AccountHeadId)
                .Index(t => t.AccountHeadId);
            
            CreateTable(
                "dbo.Income",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountHeadId = c.Int(nullable: false),
                        Details = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountHead", t => t.AccountHeadId)
                .Index(t => t.AccountHeadId);
            
            CreateTable(
                "dbo.Ledger",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        Income = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Expense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SaleInvoice",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                        OrderTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShippingFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentStatus = c.String(),
                        PaymentMode = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.OrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SaleInvoice", "UserId", "dbo.User");
            DropForeignKey("dbo.SaleInvoice", "OrderId", "dbo.Order");
            DropForeignKey("dbo.Income", "AccountHeadId", "dbo.AccountHead");
            DropForeignKey("dbo.Expense", "AccountHeadId", "dbo.AccountHead");
            DropIndex("dbo.SaleInvoice", new[] { "OrderId" });
            DropIndex("dbo.SaleInvoice", new[] { "UserId" });
            DropIndex("dbo.Income", new[] { "AccountHeadId" });
            DropIndex("dbo.Expense", new[] { "AccountHeadId" });
            DropTable("dbo.SaleInvoice");
            DropTable("dbo.Ledger");
            DropTable("dbo.Income");
            DropTable("dbo.Expense");
            DropTable("dbo.AccountHead");
        }
    }
}
