using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BooksLibrary.Business.Models
{
    public class Reservation
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ReservationId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ReservationDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateOfReceipt { get; set; }

        public int UserId { get; set; }
        public virtual UserModel User { get; set; }

        public int BookId { get; set; }
        public virtual Book Book { get; set; }
    }
}
