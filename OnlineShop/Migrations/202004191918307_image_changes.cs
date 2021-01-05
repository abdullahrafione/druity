namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class image_changes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Image", "IsPrimary", c => c.Boolean(nullable: false));
            AddColumn("dbo.Image", "IsSizeGuide", c => c.Boolean(nullable: false));
            AddColumn("dbo.Image", "DisplayOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Image", "DisplayOrder");
            DropColumn("dbo.Image", "IsSizeGuide");
            DropColumn("dbo.Image", "IsPrimary");
        }
    }
}
