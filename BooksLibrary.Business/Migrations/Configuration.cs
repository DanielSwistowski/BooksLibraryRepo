namespace BooksLibrary.Business.Migrations
{
    using BooksLibrary.Business.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<BooksLibrary.Business.BookLibraryDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BooksLibrary.Business.BookLibraryDataContext context)
        {
            //  This method will be called after migrating to the latest version.
            context.Categories.AddOrUpdate(c => c.CategoryName,
                new Category { CategoryName = "Kategoria 1" },
                new Category { CategoryName = "Kategoria 2" },
                new Category { CategoryName = "Kategoria 3" },
                new Category { CategoryName = "Kategoria 4" },
                new Category { CategoryName = "Kategoria 5" },
                new Category { CategoryName = "Kategoria 6" });
            context.SaveChanges();

            context.Books.AddOrUpdate(b => b.Title,
                new Book { Title = "ASP.NET MVC4 Zaawansowane programowanie", Author = "Adam Freeman", ISBN = GenerateISBN(), ReleaseDate = "2013", PublishingHouse = "Helion", Quantity = 5, CategoryId = 1 },
                new Book { Title = "C# 5.0 Programowanie", Author = "Ian Griffiths", ISBN = GenerateISBN(), ReleaseDate = "2014", PublishingHouse = "Helion", Quantity = 2, CategoryId = 1 },
                new Book { Title = "Tworzenie nowoczesnych aplikacji graficznych w WPF", Author = "Jaros³aw Cisek", ISBN = GenerateISBN(), ReleaseDate = "2012", PublishingHouse = "Helion", Quantity = 7, CategoryId = 1 },
                new Book { Title = "C# Tworzenie aplikacji sieciowych. Gotowe projekty", Author = "S³awomir Or³owski", ISBN = GenerateISBN(), ReleaseDate = "2010", PublishingHouse = "Helion", Quantity = 1, CategoryId = 1 },
                new Book { Title = "WCF od podstaw", Author = "Maciej Grabek", ISBN = GenerateISBN(), ReleaseDate = "2009", PublishingHouse = "Helion", Quantity = 3, CategoryId = 1 },
                new Book { Title = "C# od podstaw", Author = "Jan Kowalski", ISBN = GenerateISBN(), ReleaseDate = "2015", PublishingHouse = "Helion", Quantity = 4, CategoryId = 1 },
                new Book { Title = "C++. Przewodnik dla pocz¹tkuj¹cych", Author = "Alex Allain", ISBN = GenerateISBN(), ReleaseDate = "2014", PublishingHouse = "Helion", Quantity = 6, CategoryId = 1 },
                new Book { Title = "C# 6.0 i MVC 5. Tworzenie nowoczesnych portali internetowych", Author = "Krzysztof ¯ydzik", ISBN = GenerateISBN(), ReleaseDate = "2015", PublishingHouse = "Helion", Quantity = 9, CategoryId = 1 },
                new Book { Title = "LINQ to Objects w C# 4.0", Author = "Troy Magennis", ISBN = GenerateISBN(), ReleaseDate = "2012", PublishingHouse = "Helion", Quantity = 7, CategoryId = 1 },
                new Book { Title = "Tablice informatyczne. C#. Wydanie II", Author = "Krzysztof Rychlicki-Kicior", ISBN = GenerateISBN(), ReleaseDate = "2015", PublishingHouse = "Helion", Quantity = 10, CategoryId = 1 },
                new Book { Title = "C#. Leksykon", Author = "Ben Albahari", ISBN = GenerateISBN(), ReleaseDate = "2013", PublishingHouse = "Helion", Quantity = 2, CategoryId = 1 },
                new Book { Title = "Wzorce projektowe C#", Author = "Steven John Metsker", ISBN = GenerateISBN(), ReleaseDate = "2013", PublishingHouse = "Helion", Quantity = 3, CategoryId = 1 },
                new Book { Title = "C#. Praktyczny kurs. Wydanie II", Author = "Marcin Lis", ISBN = GenerateISBN(), ReleaseDate = "2009", PublishingHouse = "Helion", Quantity = 4, CategoryId = 1 },
                new Book { Title = "Warsztat programisty C# 2008", Author = "Wei-Meng Lee", ISBN = GenerateISBN(), ReleaseDate = "2008", PublishingHouse = "Helion", Quantity = 5, CategoryId = 1 },
                new Book { Title = "C#. Rusz g³ow¹!", Author = "Andrew Stellman", ISBN = GenerateISBN(), ReleaseDate = "2007", PublishingHouse = "Helion", Quantity = 6, CategoryId = 1 },
                new Book { Title = "Microsoft Visual Studio 2012. Programowanie w C#", Author = "Dawid Farbaniec", ISBN = GenerateISBN(), ReleaseDate = "2012", PublishingHouse = "Helion", Quantity = 9, CategoryId = 1 },
                new Book { Title = "Silverlight 4 w dzia³aniu", Author = "Pete Brown", ISBN = GenerateISBN(), ReleaseDate = "2014", PublishingHouse = "Helion", Quantity = 7, CategoryId = 1 },
                new Book { Title = "Spring w akcji. Wydanie IV", Author = "Craig Walls", ISBN = GenerateISBN(), ReleaseDate = "2015", PublishingHouse = "Helion", Quantity = 8, CategoryId = 1 },
                new Book { Title = "Efektywne programowanie w jêzyku Java", Author = "Joshua Bloch", ISBN = GenerateISBN(), ReleaseDate = "2010", PublishingHouse = "Helion", Quantity = 8, CategoryId = 1 },
                new Book { Title = "Java. Wzorce projektowe", Author = "James William Cooper", ISBN = GenerateISBN(), ReleaseDate = "2010", PublishingHouse = "Helion", Quantity = 2, CategoryId = 1 },
                new Book { Title = "Java. Tworzenie gier", Author = "David Brackeen", ISBN = GenerateISBN(), ReleaseDate = "2015", PublishingHouse = "Helion", Quantity = 3, CategoryId = 1 },
                new Book { Title = "Java. Efektywne programowanie. Wydanie II", Author = "Joshua Bloch", ISBN = GenerateISBN(), ReleaseDate = "2013", PublishingHouse = "Helion", Quantity = 5, CategoryId = 1 },
                new Book { Title = "Java. Podstawy. Wydanie VIII", Author = "Cay S. Horstmann", ISBN = GenerateISBN(), ReleaseDate = "2012", PublishingHouse = "Helion", Quantity = 1, CategoryId = 1 },
                new Book { Title = "Algorytmy i struktury danych", Author = "Alfred V. Aho", ISBN = GenerateISBN(), ReleaseDate = "2008", PublishingHouse = "Helion", Quantity = 4, CategoryId = 1 },
                new Book { Title = "Algorytmy. Æwiczenia", Author = "Bogdan Buczek", ISBN = GenerateISBN(), ReleaseDate = "2009", PublishingHouse = "Helion", Quantity = 4, CategoryId = 1 }
            );


            var users = new List<UserModel>()
            {
                new UserModel { UserName="DanielSwistowski", Gender=Gender.Male, FirstName="Daniel", LastName="Œwistowski", Email="danielswistowski@wp.pl", UserIsEnabled=true },
                new UserModel { UserName="JanKowalski", Gender=Gender.Male,FirstName="Jan", LastName="Kowalski", Email="jankowalski@wp.pl", UserIsEnabled=true },
                new UserModel { UserName="BenjaminFranklin", Gender=Gender.Male,FirstName="Benjamin", LastName="Franklin", Email="benjaminfranklin@wp.pl", UserIsEnabled=true },
                new UserModel { UserName="BridgetJohns", Gender=Gender.Female,FirstName="Briget", LastName="Johns", Email="brigetjohns@wp.pl" , UserIsEnabled=true},
                new UserModel { UserName="AdamFreeman", Gender=Gender.Male,FirstName="Adam", LastName="Freeman", Email="adamfreeman@wp.pl" , UserIsEnabled=true},
                new UserModel { UserName="AnnaNowak", Gender=Gender.Female,FirstName="Anna", LastName="Nowak", Email="annanowak@wp.pl" , UserIsEnabled=true},
                new UserModel { UserName="TomaszLato", Gender=Gender.Male,FirstName="Tomasz", LastName="Lato", Email="tomaszlato@wp.pl" , UserIsEnabled=true}
            };

            WebSecurity.InitializeDatabaseConnection("BookLibraryDbConnectionString", "UserModel", "UserId", "UserName", autoCreateTables: true);
            foreach (var user in users)
            {
                if (!WebSecurity.UserExists(user.UserName))
                {
                    var token = WebSecurity.CreateUserAndAccount(user.UserName, "haslo", new { user.FirstName, user.LastName, user.Email, user.Gender, user.UserIsEnabled }, true);
                    WebSecurity.ConfirmAccount(user.UserName, token);
                }
            }

            if (!Roles.RoleExists("Admin"))
            {
                Roles.CreateRole("Admin");
            }
            if (!Roles.RoleExists("U¿ytkownik"))
            {
                Roles.CreateRole("U¿ytkownik");
            }
            if (!Roles.IsUserInRole("DanielSwistowski", "Admin"))
            {
                Roles.AddUserToRole("DanielSwistowski", "Admin");
            }
        }

        private Random random = new Random();
        private string GenerateISBN()
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPRSTUQWXYZ";
            return new string(Enumerable.Repeat(chars, 12)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
