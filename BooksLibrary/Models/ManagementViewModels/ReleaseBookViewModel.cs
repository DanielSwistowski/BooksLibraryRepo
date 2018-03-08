using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.ReservationViewModels
{
    public class ReleaseBookViewModel
    {
        [ScaffoldColumn(false)]
        public int ReservationID { get; set; }

        [Display(Name = "Tytył książki")]
        public string Title { get; set; }

        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Display(Name = "Wypożyczający")]
        public string UserFullName { get; set; }

        [Display(Name = "Okres wypożyczenia")]
        [Required(ErrorMessage = "Wybierz liczbę dni")]
        [Range(1, 10, ErrorMessage = "Niepoprawna liczba dni")]
        public int RentTimeInDays { get; set; }
    }
}