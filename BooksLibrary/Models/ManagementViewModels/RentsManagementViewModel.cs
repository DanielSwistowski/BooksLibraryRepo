using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.ManagementViewModels
{
    public class RentsManagementViewModel
    {
        [ScaffoldColumn(false)]
        public int RentId { get; set; }

        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        [ScaffoldColumn(false)]
        public int BookId { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name="Termin zwrotu")]
        public DateTime ReturnDate { get; set; }

        [Display(Name="Wypożyczający")]
        public string UserFullName { get; set; }

        [Display(Name="Książka")]
        public string Title { get; set; }

        [Display(Name="Autor")]
        public string Author { get; set; }
    }
}