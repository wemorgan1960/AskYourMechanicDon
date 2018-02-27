namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "VIN", c => c.String());
            AddColumn("dbo.Products", "Question", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Question");
            DropColumn("dbo.Products", "VIN");
        }
    }
}
