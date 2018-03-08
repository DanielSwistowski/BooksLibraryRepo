using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Aktualne hasło")]
        [Required(ErrorMessage = "Wpisz swoje aktualne hasło")]
        public string ActualPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        [Required(ErrorMessage = "Wpisz nowe hasło")]
        [StringLength(100, ErrorMessage = "Hasło musi mieć przynajmniej {2} znaków", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Powtórz hasło")]
        [Compare("NewPassword", ErrorMessage = "Podane hasła się nie zgadzają")]
        public string NewPasswordConfirm { get; set; }
    }
}