namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gendertag : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GenderTag",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Product", "GenderTagId", c => c.Int(nullable: false));
            CreateIndex("dbo.Product", "GenderTagId");
            AddForeignKey("dbo.Product", "GenderTagId", "dbo.GenderTag", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Product", "GenderTagId", "dbo.GenderTag");
            DropIndex("dbo.Product", new[] { "GenderTagId" });
            DropColumn("dbo.Product", "GenderTagId");
            DropTable("dbo.GenderTag");
        }
    }
}
