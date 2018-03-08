using BooksLibrary.Business.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business
{
    public class BookLibraryDataContext : DbContext
    {
        public BookLibraryDataContext()
            : base("BookLibraryDbConnectionString")
        { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Rent> Rents { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<LockAccountReason> LockAccountsReasons { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}