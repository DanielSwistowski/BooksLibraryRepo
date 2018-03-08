using AutoMapper;
using BooksLibrary.Business.Models;
using BooksLibrary.Business.SecurityServices;
using BooksLibrary.Business.Service;
using BooksLibrary.Models.ManagementViewModels;
using BooksLibrary.Models.ReservationViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace BooksLibrary.Controllers
{
    public class ManagementController : Controller
    {
        private readonly IReservationService reservationService;
        private readonly IManagementService managementService;
        private readonly IBookService bookService;
        private readonly IRentService rentService;
        private readonly IMapper mapper;
        public ManagementController(IReservationService reservationService, IManagementService managementService, IBookService bookService, IRentService rentService, IMapper mapper)
        {
            this.reservationService = reservationService;
            this.managementService = managementService;
            this.bookService = bookService;
            this.rentService = rentService;
            this.mapper = mapper;
        }

        [ChildActionOnly]
        public ActionResult ManagementPanel()
        {
            if (User.IsInRole("Admin"))
            {
                return PartialView("_ManagementPartial");
            }
            return new EmptyResult();
        }


        public ActionResult ReservationsManagement(int? page, string searchUserFirstName, string searchUserLastName, string searchBook)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            int totalCount;

            ViewBag.SearchUserFirstName = searchUserFirstName;
            ViewBag.SearchUserLastName = searchUserLastName;
            ViewBag.SearchBook = searchBook;

            string[] includedProperties = new string[] { "Book", "User" };
            Func<IQueryable<Reservation>, IOrderedQueryable<Reservation>> orderBy = o => o.OrderBy(r => r.ReservationDate);

            IEnumerable<Reservation> query = null;

            if (!string.IsNullOrEmpty(searchBook) && !string.IsNullOrEmpty(searchUserFirstName) && !string.IsNullOrEmpty(searchUserLastName))
            {
                query = reservationService.GetAllReservations(r => r.User.FirstName.ToUpper().Contains(searchUserFirstName.ToUpper())
                    && r.User.LastName.ToUpper().Contains(searchUserLastName.ToUpper())
                    && r.Book.Title.ToUpper().Contains(searchBook.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchBook) && !string.IsNullOrEmpty(searchUserFirstName))
            {
                query = reservationService.GetAllReservations(r => r.User.FirstName.ToUpper().Contains(searchUserFirstName.ToUpper())
                    && r.Book.Title.ToUpper().Contains(searchBook.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchBook) && !string.IsNullOrEmpty(searchUserLastName))
            {
                query = reservationService.GetAllReservations(r => r.User.LastName.ToUpper().Contains(searchUserLastName.ToUpper())
                    && r.Book.Title.ToUpper().Contains(searchBook.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchUserFirstName) && !string.IsNullOrEmpty(searchUserLastName))
            {
                query = reservationService.GetAllReservations((r => r.User.FirstName.ToUpper().Contains(searchUserFirstName.ToUpper())
                    && r.User.LastName.ToUpper().Contains(searchUserLastName.ToUpper())), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchBook))
            {
                query = reservationService.GetAllReservations(r => r.Book.Title.ToUpper().Contains(searchBook.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchUserFirstName))
            {
                query = reservationService.GetAllReservations(r => r.User.FirstName.ToUpper().Contains(searchUserFirstName.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchUserLastName))
            {
                query = reservationService.GetAllReservations(r => r.User.LastName.ToUpper().Contains(searchUserLastName.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else
            {
                query = reservationService.GetAllReservations(null, includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }

            IEnumerable<ReservationsManagementViewModel> reservationsModel = mapper.Map<IEnumerable<ReservationsManagementViewModel>>(query);
            var reservationsPaged = new StaticPagedList<ReservationsManagementViewModel>(reservationsModel, pageNumber, pageSize, totalCount);

            return View(reservationsPaged);
        }


        public ActionResult RentsManagement(int? page, string searchUserFirstName, string searchUserLastName, string searchBook)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            int totalCount;

            ViewBag.SearchUserFirstName = searchUserFirstName;
            ViewBag.SearchUserLastName = searchUserLastName;
            ViewBag.SearchBook = searchBook;

            string[] includedProperties = new string[] { "Book", "User" };
            Func<IQueryable<Rent>, IOrderedQueryable<Rent>> orderBy = o => o.OrderBy(r => r.ReturnDate);

            IEnumerable<Rent> query = null;
            if (!string.IsNullOrEmpty(searchBook) && !string.IsNullOrEmpty(searchUserFirstName) && !string.IsNullOrEmpty(searchUserLastName))
            {
                query = rentService.GetAllRents(r => r.User.FirstName.ToUpper().Contains(searchUserFirstName.ToUpper())
                    && r.User.LastName.ToUpper().Contains(searchUserLastName.ToUpper())
                    && r.Book.Title.ToUpper().Contains(searchBook.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchBook) && !string.IsNullOrEmpty(searchUserFirstName))
            {
                query = rentService.GetAllRents(r => r.User.FirstName.ToUpper().Contains(searchUserFirstName.ToUpper())
                    && r.Book.Title.ToUpper().Contains(searchBook.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchBook) && !string.IsNullOrEmpty(searchUserLastName))
            {
                query = rentService.GetAllRents(r => r.User.LastName.ToUpper().Contains(searchUserLastName.ToUpper())
                    && r.Book.Title.ToUpper().Contains(searchBook.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchUserFirstName) && !string.IsNullOrEmpty(searchUserLastName))
            {
                query = rentService.GetAllRents((r => r.User.FirstName.ToUpper().Contains(searchUserFirstName.ToUpper())
                    && r.User.LastName.ToUpper().Contains(searchUserLastName.ToUpper())), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchBook))
            {
                query = rentService.GetAllRents(r => r.Book.Title.ToUpper().Contains(searchBook.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchUserFirstName))
            {
                query = rentService.GetAllRents(r => r.User.FirstName.ToUpper().Contains(searchUserFirstName.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(searchUserLastName))
            {
                query = rentService.GetAllRents(r => r.User.LastName.ToUpper().Contains(searchUserLastName.ToUpper()), includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }
            else
            {
                query = rentService.GetAllRents(null, includedProperties, orderBy, out totalCount, pageNumber, pageSize);
            }

            IEnumerable<RentsManagementViewModel> rentsModel = mapper.Map<IEnumerable<RentsManagementViewModel>>(query);
            var rentsPaged = new StaticPagedList<RentsManagementViewModel>(rentsModel, pageNumber, pageSize, totalCount);
            return View(rentsPaged);
        }


        public ActionResult ReleaseBook(int reservationId)
        {
            Reservation reservation = reservationService.FindReservation(reservationId);

            if (reservation == null)
                return HttpNotFound();

            ReleaseBookViewModel model = mapper.Map<ReleaseBookViewModel>(reservation);
            PopulateRentDaysDropDownList();
            return View(model);
        }

        private void PopulateRentDaysDropDownList()
        {
            List<SelectListItem> Days = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Wybierz...", Value = "0" }
            };
            for (int i = 1; i <= 10; i++)
            {
                Days.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            ViewBag.RentDays = Days;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReleaseBook(ReleaseBookViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                bool success = managementService.ReleaseBook(model.ReservationID, model.RentTimeInDays);
                if (success)
                {
                    TempData["message"] = "Książka została wypożyczona!";
                }
                else
                {
                    ModelState.AddModelError("", "Rezerwacja książki została usunięta!");
                    PopulateRentDaysDropDownList();
                    return View(model);
                }
                return RedirectToReferrerUrl(returnUrl, "ReservationsManagement");
            }
            PopulateRentDaysDropDownList();
            return View(model);
        }

        public ActionResult ReturnBook(int rentId)
        {
            Rent rent = rentService.FindRent(rentId);

            if (rent == null)
                return HttpNotFound();

            ReturnBookViewModel model = mapper.Map<ReturnBookViewModel>(rent);

            return View(model);
        }

        [HttpPost, ActionName("ReturnBook")]
        [ValidateAntiForgeryToken]
        public ActionResult ReturnBookConfirm(int rentId, int userId, string returnUrl)
        {
            managementService.ReturnBook(rentId);
            if (managementService.ShouldEnableUserAccount(userId))
            {
                return RedirectToAction("EnableUserAccount", "Account", new { userId = userId });
            }
            else
            {
                TempData["message"] = "Książka została zwrócona!";
                return RedirectToReferrerUrl(returnUrl, "RentsManagement");
            }
        }

        [AllowAnonymous]
        public ActionResult RedirectToReferrerUrl(string returnUrl, string baseAction)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(baseAction);
        }

        protected override void Dispose(bool disposing)
        {
            reservationService.Dispose();
            managementService.Dispose();
            bookService.Dispose();
            rentService.Dispose();
            base.Dispose(disposing);
        }
    }
}
