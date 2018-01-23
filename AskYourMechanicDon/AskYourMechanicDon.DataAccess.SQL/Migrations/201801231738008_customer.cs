namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Province", c => c.String());
            AddColumn("dbo.Customers", "PostalCode", c => c.String());
            AddColumn("dbo.Orders", "Province", c => c.String());
            AddColumn("dbo.Orders", "PostalCode", c => c.String());
            DropColumn("dbo.Customers", "State");
            DropColumn("dbo.Customers", "ZipCode");
            DropColumn("dbo.Orders", "State");
            DropColumn("dbo.Orders", "ZipCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "ZipCode", c => c.String());
            AddColumn("dbo.Orders", "State", c => c.String());
            AddColumn("dbo.Customers", "ZipCode", c => c.String());
            AddColumn("dbo.Customers", "State", c => c.String());
            DropColumn("dbo.Orders", "PostalCode");
            DropColumn("dbo.Orders", "Province");
            DropColumn("dbo.Customers", "PostalCode");
            DropColumn("dbo.Customers", "Province");
        }
    }
}
