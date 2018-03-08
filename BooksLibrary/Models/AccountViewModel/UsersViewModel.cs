using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.AccountViewModel
{
    public class UsersViewModel
    {
        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserFullName { get; set; }

        public string Email { get; set; }

        public bool UserIsEnabled { get; set; }
    }
}