using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Models
{
    public enum Gender
    {
        Male,
        Female
    }

    public class UserModel
    {
        public UserModel()
        {
            this.Rents = new HashSet<Rent>();
            this.Reservations = new HashSet<Reservation>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Gender Gender { get; set; }

        public string Email { get; set; }

        public bool UserIsEnabled { get; set; }

        public virtual LockAccountReason LockAccountReason { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<Rent> Rents { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}