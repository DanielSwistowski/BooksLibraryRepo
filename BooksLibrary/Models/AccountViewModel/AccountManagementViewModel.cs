using BooksLibrary.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class AccountManagementViewModel
    {
        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        [Display(Name = "Login")]
        public string UserName { get; set; }

        [Display(Name = "Imię")]
        [Required(ErrorMessage = "Imię jest wymagane")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public string LastName { get; set; }

        [Display(Name = "Płeć")]
        public Gender Gender { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "E-mail jest wymagany")]
        public string Email { get; set; }

        [Display(Name = "Aktywne konto")]
        public bool UserIsEnabled { get; set; }

        public string[] UserRoles { get; set; }

        public AddressViewModel UserAddress { get; set; }
    }
}