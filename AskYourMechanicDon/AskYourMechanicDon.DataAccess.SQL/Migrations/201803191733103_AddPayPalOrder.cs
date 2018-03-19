namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayPalOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "PayPalTxnId", c => c.String());
            AddColumn("dbo.Orders", "PayPalPaidDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "PayPalPaidDate");
            DropColumn("dbo.Orders", "PayPalTxnId");
        }
    }
}
