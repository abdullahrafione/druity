namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class product_ShortDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "ShortDescription", c => c.String());
            AlterColumn("dbo.Product", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product", "Name", c => c.String(nullable: false, maxLength: 30));
            DropColumn("dbo.Product", "ShortDescription");
        }
    }
}
