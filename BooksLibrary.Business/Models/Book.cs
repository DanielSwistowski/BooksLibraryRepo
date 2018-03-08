using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BooksLibrary.Business.Models
{
    public class Book
    {
        public Book()
        {
            this.Rents = new HashSet<Rent>();
            this.Reservations = new HashSet<Reservation>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string ISBN { get; set; }

        public string PublishingHouse { get; set; }

        public string ReleaseDate { get; set; }

        public int Quantity { get; set; }

        public int CategoryId { get; set; }
        public virtual Category BookCategory { get; set; }

        public virtual ICollection<Rent> Rents { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
