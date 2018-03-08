using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.ReservationViewModels
{
    public class ReservationViewModel
    {
        [ScaffoldColumn(false)]
        public int ReservationId { get; set; }

        [Display(Name = "Książka")]
        public string Title { get; set; }

        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Display(Name="Data rezerwacji")]
        [DataType(DataType.DateTime)]
        public DateTime ReservationDate { get; set; }

        [Display(Name = "Termin odbioru")]
        [DataType(DataType.DateTime)]
        public DateTime DateOfReceipt { get; set; }
    }
}