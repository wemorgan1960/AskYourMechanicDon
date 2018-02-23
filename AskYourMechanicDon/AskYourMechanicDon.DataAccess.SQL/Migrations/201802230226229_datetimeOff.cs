namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetimeOff : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "CompletedAt", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "CompletedAt", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
    }
}
