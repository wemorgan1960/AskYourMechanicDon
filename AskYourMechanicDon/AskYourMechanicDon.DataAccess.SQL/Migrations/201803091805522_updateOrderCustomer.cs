namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrderCustomer : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.Orders", new[] { "Customer_Id" });
            DropColumn("dbo.Orders", "OrderId");
            DropColumn("dbo.Orders", "Customer_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "Customer_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Orders", "OrderId", c => c.String());
            CreateIndex("dbo.Orders", "Customer_Id");
            AddForeignKey("dbo.Orders", "Customer_Id", "dbo.Customers", "Id");
        }
    }
}
