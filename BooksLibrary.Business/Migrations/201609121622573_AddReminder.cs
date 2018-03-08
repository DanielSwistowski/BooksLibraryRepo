namespace BooksLibrary.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReminder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reminder",
                c => new
                    {
                        RentId = c.Int(nullable: false),
                        ReminderWasSent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RentId)
                .ForeignKey("dbo.Rent", t => t.RentId)
                .Index(t => t.RentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reminder", "RentId", "dbo.Rent");
            DropIndex("dbo.Reminder", new[] { "RentId" });
            DropTable("dbo.Reminder");
        }
    }
}
