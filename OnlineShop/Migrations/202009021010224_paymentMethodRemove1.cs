namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentMethodRemove1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Payment", "PaymentMethodId", "dbo.PaymentMethod");
            DropIndex("dbo.Payment", new[] { "PaymentMethodId" });
            DropColumn("dbo.Payment", "PaymentMethodId");
            DropTable("dbo.PaymentMethod");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PaymentMethod",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Payment", "PaymentMethodId", c => c.Int(nullable: false));
            CreateIndex("dbo.Payment", "PaymentMethodId");
            AddForeignKey("dbo.Payment", "PaymentMethodId", "dbo.PaymentMethod", "Id");
        }
    }
}
