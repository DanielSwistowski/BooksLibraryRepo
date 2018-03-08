using AutoMapper;
using BooksLibrary.Business.Models;
using BooksLibrary.Business.SecurityServices;
using BooksLibrary.Business.Service;
using BooksLibrary.Models.BookViewModels;
using BooksLibrary.Models.ReservationViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace BooksLibrary.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly IReservationService reservationService;
        private readonly IMapper mapper;
        private readonly IBookService bookService;
        private readonly IRentService rentService;
        private readonly IUserService userService;
        public ReservationController(IReservationService service, IMapper map, IBookService bookSer, IRentService rentS, IUserService userSer)
        {
            reservationService = service;
            mapper = map;
            bookService = bookSer;
            rentService = rentS;
            userService = userSer;
        }

        public ActionResult IndexUserReservations(int? bookId)
        {
            if (bookId != null)
            {
                ViewBag.BookID = bookId;
                ModelState.AddModelError("", "Wybrana książka jest już przez Ciebie zarezerwowana!");
            }

            int userId = userService.CurrentUserId();
            IEnumerable<Reservation> reservations = reservationService.GetAllReservations(u => u.UserId == userId, new string[] {"Book"});

            IEnumerable<UserReservationsViewModel> allUserReservations = mapper.Map<IEnumerable<UserReservationsViewModel>>(reservations);

            return View(allUserReservations);
        }

        public ActionResult AddReservation(int bookId)
        {
            Book book = bookService.FindBook(bookId);

            if (book == null)
                return HttpNotFound();

            if (book.Quantity == 0)
                return RedirectToAction("BookIsNotAvailable", "Book", new { bookId = bookId });

            BookViewModel bookModel = mapper.Map<BookViewModel>(book);
            return View(bookModel);
        }

        [HttpPost,ActionName("AddReservation")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmReservation(int bookId, string returnUrl)
        {
            int userId = userService.CurrentUserId();
            Reservation reservation = new Reservation();
            reservation.UserId = userId;
            reservation.BookId = bookId;
            reservation.ReservationDate = DateTime.Now;
            reservation.DateOfReceipt = DateTime.Now.AddDays(1);

            if (!userService.UserAddressExists(userId))
                return RedirectToAction("AddAddress", "Account");

            if(reservationService.ReservationExists(userId,bookId))
                return RedirectToAction("IndexUserReservations", new { bookId = bookId });

            if(rentService.RentExists(userId,bookId))
                return RedirectToAction("IndexUserRents", "Rent", new { bookId = bookId });

            if (!bookService.BookIsAvailable(bookId))
                return RedirectToAction("BookIsNotAvailable", "Book", new { bookId = bookId });

            reservationService.AddReservation(reservation);
            TempData["message"] = "Książka została zarezerwowana!";
            return RedirectToReferrerUrl(returnUrl);
        }

        public ActionResult DeleteReservation(int reservationId)
        {
            Reservation reservation = reservationService.FindReservation(reservationId);

            if (reservation == null)
                return HttpNotFound();

            ReservationViewModel reservationModel = mapper.Map<ReservationViewModel>(reservation);

            return View(reservationModel);
        }

        [HttpPost, ActionName("DeleteReservation")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteReservationConfirm(int reservationId)
        {
            Reservation reservation = reservationService.FindReservation(reservationId);
            if (reservation == null)
            {
                TempData["message"] = "Rezerwacja nie istnieje lub została już anulowana!";
            }
            else
            {
                reservationService.DeleteReservation(reservationId);
                TempData["message"] = "Rezerwacja została anulowana!";
            }
            return RedirectToAction("IndexUserReservations");
        }

        [AllowAnonymous]
        public ActionResult RedirectToReferrerUrl(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Book");
        }

        protected override void Dispose(bool disposing)
        {
            reservationService.Dispose();
            bookService.Dispose();
            rentService.Dispose();
            base.Dispose(disposing);
        }
    }
}
