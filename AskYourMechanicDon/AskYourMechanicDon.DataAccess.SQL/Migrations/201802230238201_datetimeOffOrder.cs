namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetimeOffOrder : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "CompletedAt", c => c.DateTimeOffset(nullable: true, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "CompletedAt", c => c.DateTime(nullable: false));
        }
    }
}
