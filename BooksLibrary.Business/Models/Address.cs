using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BooksLibrary.Business.Models
{
    public class Address
    {
        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual UserModel User { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }

        public string ZipCode { get; set; }
    }
}
