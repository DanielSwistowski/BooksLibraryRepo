using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class GetEmailAddressViewModel
    {
        [Display(Name = "Twój adres e-mail")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Niepoprawny adres email")]
        [Required(ErrorMessage = "Adres email jest wymagany")]
        public string Email { get; set; }
    }
}