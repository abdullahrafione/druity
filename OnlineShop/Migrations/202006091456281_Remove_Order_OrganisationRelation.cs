namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Order_OrganisationRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Order", "OrganisationId", "dbo.Organisation");
            DropIndex("dbo.Order", new[] { "OrganisationId" });
            DropColumn("dbo.Order", "OrganisationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Order", "OrganisationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Order", "OrganisationId");
            AddForeignKey("dbo.Order", "OrganisationId", "dbo.Organisation", "Id", cascadeDelete: true);
        }
    }
}
