namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guidinorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "Guid", c => c.String());
            DropColumn("dbo.Order", "Note");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Order", "Note", c => c.String());
            DropColumn("dbo.Order", "Guid");
        }
    }
}
