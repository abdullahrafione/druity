namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
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
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Tag = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        ParentCategoryId = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ShortDescription = c.String(),
                        Detail = c.String(),
                        CategoryId = c.Int(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                        GenderTagId = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.GenderTag", t => t.GenderTagId, cascadeDelete: true)
                .ForeignKey("dbo.Organisation", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.OrganisationId)
                .Index(t => t.GenderTagId);
            
            CreateTable(
                "dbo.GenderTag",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Image",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Url = c.String(),
                        ProductId = c.Int(nullable: false),
                        IsPrimary = c.Boolean(nullable: false),
                        IsSizeGuide = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Organisation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        RoleName = c.String(nullable: false, maxLength: 20),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 30),
                        LastName = c.String(nullable: false, maxLength: 30),
                        EmailAddress = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false),
                        Phone = c.String(nullable: false, maxLength: 20),
                        Street = c.String(nullable: false, maxLength: 50),
                        PostalCode = c.String(nullable: false, maxLength: 20),
                        Address = c.String(nullable: false, maxLength: 400),
                        City = c.String(nullable: false, maxLength: 50),
                        State = c.String(nullable: false, maxLength: 50),
                        Country = c.String(nullable: false, maxLength: 50),
                        UniqueId = c.String(),
                        OrganisationId = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisation", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Guid = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.OrderDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ProductStockId = c.Int(nullable: false),
                        OrderStatusId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitSalePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.OrderStatus", t => t.OrderStatusId, cascadeDelete: true)
                .ForeignKey("dbo.ProductStock", t => t.ProductStockId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductStockId)
                .Index(t => t.OrderStatusId);
            
            CreateTable(
                "dbo.OrderStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderStatusLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Description = c.String(),
                        DateTime = c.DateTime(nullable: false),
                        OrderDetailId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderDetail", t => t.OrderDetailId, cascadeDelete: true)
                .Index(t => t.OrderDetailId);
            
            CreateTable(
                "dbo.ProductStock",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrentPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OldPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cost = c.Decimal(precision: 18, scale: 2),
                        OnSale = c.Boolean(nullable: false),
                        IsFeatured = c.Boolean(nullable: false),
                        TopRated = c.Boolean(nullable: false),
                        StockCount = c.Int(nullable: false),
                        CurrencySymbol = c.String(),
                        ColourId = c.Int(nullable: false),
                        SizeID = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Colour", t => t.ColourId, cascadeDelete: true)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .ForeignKey("dbo.Size", t => t.SizeID, cascadeDelete: true)
                .Index(t => t.ColourId)
                .Index(t => t.SizeID)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Colour",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Size",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaymentStatusId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentId = c.String(),
                        CurrencyCode = c.String(),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.PaymentStatus", t => t.PaymentStatusId, cascadeDelete: true)
                .Index(t => t.PaymentStatusId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.PaymentStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
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
            DropForeignKey("dbo.Product", "OrganisationId", "dbo.Organisation");
            DropForeignKey("dbo.User", "OrganisationId", "dbo.Organisation");
            DropForeignKey("dbo.Order", "UserId", "dbo.User");
            DropForeignKey("dbo.Payment", "PaymentStatusId", "dbo.PaymentStatus");
            DropForeignKey("dbo.Payment", "OrderId", "dbo.Order");
            DropForeignKey("dbo.OrderDetail", "ProductStockId", "dbo.ProductStock");
            DropForeignKey("dbo.ProductStock", "SizeID", "dbo.Size");
            DropForeignKey("dbo.ProductStock", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductStock", "ColourId", "dbo.Colour");
            DropForeignKey("dbo.OrderStatusLogs", "OrderDetailId", "dbo.OrderDetail");
            DropForeignKey("dbo.OrderDetail", "OrderStatusId", "dbo.OrderStatus");
            DropForeignKey("dbo.OrderDetail", "OrderId", "dbo.Order");
            DropForeignKey("dbo.Image", "ProductId", "dbo.Product");
            DropForeignKey("dbo.Product", "GenderTagId", "dbo.GenderTag");
            DropForeignKey("dbo.Product", "CategoryId", "dbo.Category");
            DropIndex("dbo.SaleInvoice", new[] { "OrderId" });
            DropIndex("dbo.SaleInvoice", new[] { "UserId" });
            DropIndex("dbo.Income", new[] { "AccountHeadId" });
            DropIndex("dbo.Expense", new[] { "AccountHeadId" });
            DropIndex("dbo.Payment", new[] { "OrderId" });
            DropIndex("dbo.Payment", new[] { "PaymentStatusId" });
            DropIndex("dbo.ProductStock", new[] { "ProductId" });
            DropIndex("dbo.ProductStock", new[] { "SizeID" });
            DropIndex("dbo.ProductStock", new[] { "ColourId" });
            DropIndex("dbo.OrderStatusLogs", new[] { "OrderDetailId" });
            DropIndex("dbo.OrderDetail", new[] { "OrderStatusId" });
            DropIndex("dbo.OrderDetail", new[] { "ProductStockId" });
            DropIndex("dbo.OrderDetail", new[] { "OrderId" });
            DropIndex("dbo.Order", new[] { "UserId" });
            DropIndex("dbo.User", new[] { "OrganisationId" });
            DropIndex("dbo.Image", new[] { "ProductId" });
            DropIndex("dbo.Product", new[] { "GenderTagId" });
            DropIndex("dbo.Product", new[] { "OrganisationId" });
            DropIndex("dbo.Product", new[] { "CategoryId" });
            DropTable("dbo.SaleInvoice");
            DropTable("dbo.Ledger");
            DropTable("dbo.Income");
            DropTable("dbo.Expense");
            DropTable("dbo.PaymentStatus");
            DropTable("dbo.Payment");
            DropTable("dbo.Size");
            DropTable("dbo.Colour");
            DropTable("dbo.ProductStock");
            DropTable("dbo.OrderStatusLogs");
            DropTable("dbo.OrderStatus");
            DropTable("dbo.OrderDetail");
            DropTable("dbo.Order");
            DropTable("dbo.User");
            DropTable("dbo.Organisation");
            DropTable("dbo.Image");
            DropTable("dbo.GenderTag");
            DropTable("dbo.Product");
            DropTable("dbo.Category");
            DropTable("dbo.AccountHead");
        }
    }
}
