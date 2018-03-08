using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class SendEmailToUserViewModel
    {
        [UIHint("ReadOnly")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Adres e-mail użytkownika")]
        public string Email { get; set; }

        [Display(Name = "Temat widomości")]
        [Required(ErrorMessage = "Wpisz temat wiadomości")]
        public string MessageSubject { get; set; }

        [Display(Name = "Treść wiadomości")]
        [Required(ErrorMessage = "Uzupełnij treść wiadomości")]
        public string Message { get; set; }
    }
}