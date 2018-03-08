using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.RoleViewModels
{
    public class AddOrDeleteUserRoleViewModel
    {
        [Display(Name = "Użytkownik")]
        public string UserName { get; set; }

        [Display(Name = "Uprawnienie")]
        [Required(ErrorMessage = "Wybierz nowe uprawnienie")]
        public string RoleName { get; set; }
    }
}