namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modelOrderItemCreatedAt : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "CreatedAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "CreatedAt", c => c.DateTime(nullable: false));
        }
    }
}
