namespace AskYourMechanicDon.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAnswer : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Answers", "OrderItemId", "dbo.OrderItems");
            DropIndex("dbo.Answers", new[] { "OrderItemId" });
            AddColumn("dbo.OrderItems", "Vin", c => c.String());
            AddColumn("dbo.OrderItems", "Question", c => c.String());
            AddColumn("dbo.OrderItems", "AnswerSubject", c => c.String());
            AddColumn("dbo.OrderItems", "AnswerContent", c => c.String());
            AddColumn("dbo.OrderItems", "AnswerTags", c => c.String());
            AddColumn("dbo.OrderItems", "AnswerCompleted", c => c.DateTime(nullable: false));
            DropColumn("dbo.OrderItems", "Image");
            DropTable("dbo.Answers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OrderItemId = c.String(maxLength: 128),
                        AnswerSubject = c.String(),
                        AnswerContent = c.String(),
                        Tags = c.String(),
                        Completed = c.DateTime(nullable: false),
                        CreateAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.OrderItems", "Image", c => c.String());
            DropColumn("dbo.OrderItems", "AnswerCompleted");
            DropColumn("dbo.OrderItems", "AnswerTags");
            DropColumn("dbo.OrderItems", "AnswerContent");
            DropColumn("dbo.OrderItems", "AnswerSubject");
            DropColumn("dbo.OrderItems", "Question");
            DropColumn("dbo.OrderItems", "Vin");
            CreateIndex("dbo.Answers", "OrderItemId");
            AddForeignKey("dbo.Answers", "OrderItemId", "dbo.OrderItems", "Id");
        }
    }
}
