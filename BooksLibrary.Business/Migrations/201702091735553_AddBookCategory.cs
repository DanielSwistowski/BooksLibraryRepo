namespace BooksLibrary.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBookCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            AddColumn("dbo.Book", "CategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Book", "CategoryId");
            AddForeignKey("dbo.Book", "CategoryId", "dbo.Category", "CategoryId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Book", "CategoryId", "dbo.Category");
            DropIndex("dbo.Book", new[] { "CategoryId" });
            DropColumn("dbo.Book", "CategoryId");
            DropTable("dbo.Category");
        }
    }
}
