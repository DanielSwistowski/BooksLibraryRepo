using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Models
{
    public class LockAccountReason
    {
        public string Reason { get; set; }

        public bool ReturnBookDateExpired { get; set; }

        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual UserModel User { get; set; }
    }
}
