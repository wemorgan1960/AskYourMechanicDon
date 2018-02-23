namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeCompletedAtOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "CompletedAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "CompletedAt", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
    }
}
