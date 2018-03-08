using AutoMapper;
using BooksLibrary.Business.Models;
using BooksLibrary.Business.SecurityServices;
using BooksLibrary.Business.Service;
using BooksLibrary.Models.RentViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace BooksLibrary.Controllers
{
    [Authorize]
    public class RentController : Controller
    {
        private readonly IRentService rentService;
        private readonly IMapper mapper;
        private readonly IUserService userService;
        public RentController(IRentService service, IMapper map, IUserService userSer)
        {
            rentService = service;
            mapper = map;
            userService = userSer;
        }

        public ActionResult IndexUserRents(int? bookId)
        {
            if (bookId != null)
            {
                ViewBag.BookID = bookId;
                ModelState.AddModelError("", "Wybrana książka jest już przez Ciebie wypożyczona!");
            }

            int userId = userService.CurrentUserId();

            IEnumerable<Rent> rents = rentService.GetAllRents(u => u.UserId == userId, new string[] { "Book" });

            IEnumerable<UserRentsViewModel> allUserRents = mapper.Map<IEnumerable<UserRentsViewModel>>(rents);

            return View(allUserRents);
        }
    }
}
