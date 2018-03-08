using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.ManagementViewModels
{
    public class ReturnBookViewModel
    {
        [ScaffoldColumn(false)]
        public int RentId { get; set; }

        [Display(Name = "Tytył książki")]
        public string Title { get; set; }

        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Display(Name = "Wypożyczający")]
        public string UserFullName { get; set; }

        [Display(Name = "Termin zwrotu")]
        [DataType(DataType.DateTime)]
        public DateTime ReturnDate { get; set; }
    }
}