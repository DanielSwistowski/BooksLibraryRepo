using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class EnableOrDisableAccountViewModel
    {
        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        [Display(Name="Nazwa użytkownika")]
        public string UserName { get; set; }

        [Display(Name="Powód blokady konta")]
        [Required(ErrorMessage="Podaj powód blokady konta użytkownika")]
        [StringLength(200,ErrorMessage="Wpisany powód blokady konta jest za długi! Limit to 200 znaków")]
        public string LockReason { get; set; }
    }
}