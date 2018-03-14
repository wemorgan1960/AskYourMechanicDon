namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ErrorRequest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Errors",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Message = c.String(),
                        CreateAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Message = c.String(),
                        CreateAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Requests");
            DropTable("dbo.Errors");
        }
    }
}
