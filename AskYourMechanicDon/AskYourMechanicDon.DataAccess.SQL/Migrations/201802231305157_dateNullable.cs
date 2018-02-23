namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateNullable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CompletedAt", c => c.DateTime());
            AlterColumn("dbo.OrderItems", "AnswerCompleted", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderItems", "AnswerCompleted", c => c.DateTime(nullable: false));
            DropColumn("dbo.Orders", "CompletedAt");
        }
    }
}
