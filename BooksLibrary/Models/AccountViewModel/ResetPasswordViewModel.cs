using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Podaj nowe hasło")]
        [Display(Name = "Nowe hasło")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Hasło musi mieć przynajmniej {2} znaków", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Display(Name = "Powtórz hasło")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Podane hasła się nie zgadzają")]
        public string NewPasswordConfirm { get; set; }

        [Required]
        public string PasswordResetToken { get; set; }
    }
}