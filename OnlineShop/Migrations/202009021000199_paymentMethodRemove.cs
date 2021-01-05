namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentMethodRemove : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Payment", "PaymentMethodId", "dbo.PaymentMethod");
            AddForeignKey("dbo.Payment", "PaymentMethodId", "dbo.PaymentMethod", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payment", "PaymentMethodId", "dbo.PaymentMethod");
            AddForeignKey("dbo.Payment", "PaymentMethodId", "dbo.PaymentMethod", "Id", cascadeDelete: true);
        }
    }
}
