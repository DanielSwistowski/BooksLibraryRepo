using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class AddressViewModel
    {
        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        [Display(Name="Miejscowość")]
        [Required(ErrorMessage="Nazwa miejscowości jest wymagana")]
        public string City { get; set; }

        [Display(Name = "Ulica")]
        [Required(ErrorMessage = "Nazwa ulicy jest wymagana")]
        public string Street { get; set; }

        [Display(Name = "Numer domu")]
        [Required(ErrorMessage = "Numer domu jest wymagany")]
        public string HouseNumber { get; set; }

        [Display(Name = "Kod pocztowy")]
        [Required(ErrorMessage = "Kod pocztowy jest wymagany")]
        [RegularExpression(@"^\d{2}-\d{3}?$", ErrorMessage = "Niepoprawny kod pocztowy")]
        public string ZipCode { get; set; }
    }
}