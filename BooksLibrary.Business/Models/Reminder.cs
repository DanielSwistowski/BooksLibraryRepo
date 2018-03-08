using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BooksLibrary.Business.Models
{
    public class Reminder
    {
        [Key]
        [ForeignKey("Rent")]
        public int RentId { get; set; }
        public virtual Rent Rent { get; set; }

        public bool ReminderWasSent { get; set; }
    }
}
