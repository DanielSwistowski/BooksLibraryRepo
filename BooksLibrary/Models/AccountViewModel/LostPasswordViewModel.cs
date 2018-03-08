using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class LostPasswordViewModel
    {
        [Display(Name = "Twój adres e-mail")]
        [Required(ErrorMessage = "Proszę podać adres e-mail")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Niepoprawny adres email")]
        public string Email { get; set; }
    }
}