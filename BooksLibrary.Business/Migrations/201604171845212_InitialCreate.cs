namespace BooksLibrary.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        City = c.String(),
                        Street = c.String(),
                        HouseNumber = c.String(),
                        ZipCode = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.UserModel", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserModel",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Gender = c.Int(nullable: false),
                        Email = c.String(),
                        UserIsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Rent",
                c => new
                    {
                        RentId = c.Int(nullable: false, identity: true),
                        RentDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RentId)
                .ForeignKey("dbo.Book", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.UserModel", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Book",
                c => new
                    {
                        BookId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Author = c.String(),
                        ISBN = c.String(),
                        PublishingHouse = c.String(),
                        ReleaseDate = c.String(),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BookId);
            
            CreateTable(
                "dbo.Reservation",
                c => new
                    {
                        ReservationId = c.Int(nullable: false, identity: true),
                        ReservationDate = c.DateTime(nullable: false),
                        DateOfReceipt = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReservationId)
                .ForeignKey("dbo.Book", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.UserModel", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.BookId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Address", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.Rent", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.Reservation", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.Reservation", "BookId", "dbo.Book");
            DropForeignKey("dbo.Rent", "BookId", "dbo.Book");
            DropIndex("dbo.Reservation", new[] { "BookId" });
            DropIndex("dbo.Reservation", new[] { "UserId" });
            DropIndex("dbo.Rent", new[] { "BookId" });
            DropIndex("dbo.Rent", new[] { "UserId" });
            DropIndex("dbo.Address", new[] { "UserId" });
            DropTable("dbo.Reservation");
            DropTable("dbo.Book");
            DropTable("dbo.Rent");
            DropTable("dbo.UserModel");
            DropTable("dbo.Address");
        }
    }
}
