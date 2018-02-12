namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderId", c => c.Double(nullable: false));
            AddColumn("dbo.Orders", "OrderNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "OrderNumber");
            DropColumn("dbo.Orders", "OrderId");
        }
    }
}
