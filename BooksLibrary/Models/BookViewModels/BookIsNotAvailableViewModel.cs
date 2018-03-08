using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.BookViewModels
{
    public class BookIsNotAvailableViewModel
    {
        public BookViewModel Book { get; set; }
        public DateTime PropablyBookAvailableDate { get; set; }
    }
}