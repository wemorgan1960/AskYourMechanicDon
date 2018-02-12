namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ordercustomer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "InvoiceNumber", c => c.String());
            AddColumn("dbo.Orders", "TotalOrder", c => c.String());
            AlterColumn("dbo.Orders", "OrderNumber", c => c.String(nullable: true));
            DropColumn("dbo.Orders", "OrderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "OrderId", c => c.Double(nullable: false));
            AlterColumn("dbo.Orders", "OrderNumber", c => c.String());
            DropColumn("dbo.Orders", "TotalOrder");
            DropColumn("dbo.Orders", "InvoiceNumber");
        }
    }
}
