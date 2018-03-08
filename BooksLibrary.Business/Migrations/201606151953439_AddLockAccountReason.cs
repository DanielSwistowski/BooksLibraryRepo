namespace BooksLibrary.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLockAccountReason : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LockAccountReason",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        Reason = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.UserModel", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LockAccountReason", "UserId", "dbo.UserModel");
            DropIndex("dbo.LockAccountReason", new[] { "UserId" });
            DropTable("dbo.LockAccountReason");
        }
    }
}
