namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderRemoveIdentity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "OrderNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "OrderNumber", c => c.String(nullable: true));
        }
    }
}
