namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CompletedAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.Orders", "FullfilledAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "FullfilledAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.Orders", "CompletedAt");
        }
    }
}
