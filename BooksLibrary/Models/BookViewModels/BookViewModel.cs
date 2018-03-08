using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.BookViewModels
{
    public class BookViewModel
    {
        [ScaffoldColumn(false)] 
        public int BookId { get; set; }

        [DisplayName("Tytuł")]
        [Required(ErrorMessage = "Tytuł jest wymagany")]
        public string Title { get; set; }

        [DisplayName("Autor")]
        [Required(ErrorMessage = "Autor jest wymagany")]
        public string Author { get; set; }

        [DisplayName("ISBN")]
        [Required(ErrorMessage = "Nr ISBN jest wymagany")]
        public string ISBN { get; set; }

        [DisplayName("Wydawnictwo")]
        [Required(ErrorMessage = "Wydawnictwo jest wymagane")]
        public string PublishingHouse { get; set; }

        [DisplayName("Kategoria")]
        [Required(ErrorMessage= "Wybierz kategorię")]
        public int CategoryId { get; set; }

        [DisplayName("Rok wydania")]
        [Required(ErrorMessage = "Rok wydania jest wymagany")]
        public string ReleaseDate { get; set; }

        [DisplayName("Ilość egzemplarzy")]
        [Range(0,int.MaxValue,ErrorMessage="Nieprawidłowa ilość egzemplarzy")]
        [Required(ErrorMessage = "Wpisz ilość egzemplarzy")]
        public int Quantity { get; set; }
    }
}