namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class answers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "FullfilledAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "Customer_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Orders", "Customer_Id");
            AddForeignKey("dbo.Orders", "Customer_Id", "dbo.Customers", "Id");
            DropColumn("dbo.Orders", "FirstName");
            DropColumn("dbo.Orders", "Surname");
            DropColumn("dbo.Orders", "Email");
            DropColumn("dbo.Orders", "Street");
            DropColumn("dbo.Orders", "City");
            DropColumn("dbo.Orders", "Province");
            DropColumn("dbo.Orders", "PostalCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "PostalCode", c => c.String());
            AddColumn("dbo.Orders", "Province", c => c.String());
            AddColumn("dbo.Orders", "City", c => c.String());
            AddColumn("dbo.Orders", "Street", c => c.String());
            AddColumn("dbo.Orders", "Email", c => c.String());
            AddColumn("dbo.Orders", "Surname", c => c.String());
            AddColumn("dbo.Orders", "FirstName", c => c.String());
            DropForeignKey("dbo.Orders", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.Orders", new[] { "Customer_Id" });
            DropColumn("dbo.Orders", "Customer_Id");
            DropColumn("dbo.Orders", "FullfilledAt");
            DropColumn("dbo.Orders", "CreatedAt");
        }
    }
}
