using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BooksLibrary.Business.Models
{
    public class Rent
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RentId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime RentDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ReturnDate { get; set; }

        public int UserId { get; set; }
        public virtual UserModel User { get; set; }

        public int BookId { get; set; }
        public virtual Book Book { get; set; }

        public virtual Reminder Reminder { get; set; }
    }
}
