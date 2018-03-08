using BooksLibrary.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BooksLibrary.Models.AccountViewModel
{
    public class EditPersonalDataViewModel
    {
        [HiddenInput(DisplayValue=false)]
        public int UserId { get; set; }

        [Display(Name = "Nazwa użytkownika")]
        [UIHint("ReadOnly")]
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
        [UIHint("ReadOnly")]
        public string Email { get; set; }

        [Display(Name = "Aktywne konto")]
        [UIHint("ReadOnly")]
        public bool UserIsEnabled { get; set; }
    }
}