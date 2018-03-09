namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrder1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CustomerUserId", c => c.String());
            AddColumn("dbo.Orders", "OrderStatusDate", c => c.DateTime());
            DropColumn("dbo.Orders", "CompletedAt");
            DropColumn("dbo.Orders", "TotalOrder");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "TotalOrder", c => c.String());
            AddColumn("dbo.Orders", "CompletedAt", c => c.DateTime());
            DropColumn("dbo.Orders", "OrderStatusDate");
            DropColumn("dbo.Orders", "CustomerUserId");
        }
    }
}
