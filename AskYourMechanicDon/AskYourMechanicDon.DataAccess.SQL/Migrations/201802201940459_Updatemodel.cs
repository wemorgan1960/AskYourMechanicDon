namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updatemodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answers", "OrderItemId", c => c.String(maxLength: 128));
            AddColumn("dbo.Customers", "Country", c => c.String());
            AddColumn("dbo.Orders", "OrderId", c => c.String());
            CreateIndex("dbo.Answers", "OrderItemId");
            AddForeignKey("dbo.Answers", "OrderItemId", "dbo.OrderItems", "Id");
            DropColumn("dbo.Orders", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "UserId", c => c.String());
            DropForeignKey("dbo.Answers", "OrderItemId", "dbo.OrderItems");
            DropIndex("dbo.Answers", new[] { "OrderItemId" });
            DropColumn("dbo.Orders", "OrderId");
            DropColumn("dbo.Customers", "Country");
            DropColumn("dbo.Answers", "OrderItemId");
        }
    }
}
