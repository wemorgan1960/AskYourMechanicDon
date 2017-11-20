namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeVinQuestionFromModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Products", "VIN");
            DropColumn("dbo.Products", "Question");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Question", c => c.String());
            AddColumn("dbo.Products", "VIN", c => c.String());
            AlterColumn("dbo.Products", "Price", c => c.String());
        }
    }
}
