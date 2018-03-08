using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksLibrary.Models.ReservationViewModels
{
    public class ReservationsManagementViewModel
    {
        [ScaffoldColumn(false)]
        public int ReservationID { get; set; }

        public string UserFullName { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ReservationDate { get; set; }
    }
}