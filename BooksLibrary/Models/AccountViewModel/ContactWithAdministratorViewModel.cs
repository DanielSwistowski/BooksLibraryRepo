using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class ContactWithAdministratorViewModel
    {
        [Display(Name="Twoja nazwa użytkownika")]
        [Required(ErrorMessage="Wpisz swoją nazwę użytkownika")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Twój adres e-mail")]
        [Required(ErrorMessage = "Adres e-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny adres email")]
        public string Email { get; set; }

        [Display(Name = "Temat widomości")]
        [Required(ErrorMessage = "Wpisz temat wiadomości")]
        public string MessageSubject { get; set; }

        [Display(Name = "Treść wiadomości")]
        [Required(ErrorMessage = "Uzupełnij treść wiadomości")]
        public string Message { get; set; }
    }
}