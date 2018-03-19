namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AmountPaidOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "AmountPaid", c => c.String());
            AddColumn("dbo.Orders", "Currency", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Currency");
            DropColumn("dbo.Orders", "AmountPaid");
        }
    }
}
