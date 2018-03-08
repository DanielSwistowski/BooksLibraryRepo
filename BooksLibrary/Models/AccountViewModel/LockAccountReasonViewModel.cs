using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class LockAccountReasonViewModel
    {
        [Display(Name="Powód blokady")]
        public string LockReason { get; set; }

        public bool ReturnBookDateExpired { get; set; }
    }
}