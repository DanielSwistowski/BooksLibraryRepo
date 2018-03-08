using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.ReservationViewModels
{
    public class UserReservationsViewModel
    {
        public int BookId { get; set; }

        public int ReservationId { get; set; }

        [Display(Name="Książka")]
        public string Title { get; set; }

        [Display(Name="Autor")]
        public string Author { get; set; }

        [Display(Name="Termin odbioru")]
        [DataType(DataType.DateTime)]
        public DateTime DateOfReceipt { get; set; }
    }
}