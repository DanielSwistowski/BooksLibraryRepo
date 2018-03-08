using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.RentViewModel
{
    public class UserRentsViewModel
    {
        [ScaffoldColumn(false)]
        public int BookId { get; set; }

        [Display(Name = "Książka")]
        public string Title { get; set; }

        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Display(Name="Data wypożyczenia")]
        [DataType(DataType.DateTime)]
        public DateTime RentDate { get; set; }

        [Display(Name="Termin zwrotu")]
        [DataType(DataType.DateTime)]
        public DateTime ReturnDate { get; set; }
    }
}