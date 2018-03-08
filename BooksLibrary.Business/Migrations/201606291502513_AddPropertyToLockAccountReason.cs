namespace BooksLibrary.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPropertyToLockAccountReason : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LockAccountReason", "ReturnBookDateExpired", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LockAccountReason", "ReturnBookDateExpired");
        }
    }
}
