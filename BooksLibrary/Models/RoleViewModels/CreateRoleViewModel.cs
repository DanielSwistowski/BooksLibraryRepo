using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.RoleViewModels
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Podaj nazwę uprawnienia")]
        [Display(Name = "Nazwa uprawnienia")]
        public string RoleName { get; set; }
    }
}