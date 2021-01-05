namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParentCategoryID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Category", "ParentCategoryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Category", "ParentCategoryId");
        }
    }
}
