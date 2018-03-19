namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "IsErrored", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "IsErrored");
        }
    }
}
