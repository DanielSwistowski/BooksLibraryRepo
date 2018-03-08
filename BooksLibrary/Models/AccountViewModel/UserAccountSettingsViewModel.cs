using BooksLibrary.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class UserAccountSettingsViewModel
    {
        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        [Display(Name = "Login")]
        public string UserName { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Płeć")]
        public Gender Gender { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Aktywne konto")]
        public bool UserIsEnabled { get; set; }

        public string[] UserRoles { get; set; }

        public AddressViewModel UserAddress { get; set; }
    }
}