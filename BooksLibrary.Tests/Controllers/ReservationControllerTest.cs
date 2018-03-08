using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BooksLibrary.Business.Service;
using AutoMapper;
using System.Collections.Generic;
using BooksLibrary.Models.ReservationViewModels;
using BooksLibrary.Business.Models;
using BooksLibrary.Controllers;
using System.Web.Mvc;
using System.Linq;
using BooksLibrary.Business.SecurityServices;

namespace BooksLibrary.Tests.Controllers
{
    [TestClass]
    public class ReservationControllerTest
    {
        Mock<IBookService> mockBookService = null;
        Mock<IReservationService> mockReservationService = null;
        Mock<IRentService> mockRentService = null;
        Mock<IMapper> mockMapper = null;
        Mock<IUserService> mockUserService = null;

        public ReservationControllerTest()
        {
            mockBookService = new Mock<IBookService>();
            mockReservationService = new Mock<IReservationService>();
            mockRentService = new Mock<IRentService>();
            mockMapper = new Mock<IMapper>();
            mockUserService = new Mock<IUserService>();
        }


        #region IndexUserReservations

        [TestMethod]
        public void IndexUserReservations_return_all_user_reservations()
        {
            mockRentService.Setup(m => m.GetAllRents());
            mockUserService.Setup(m => m.CurrentUserId());

            List<UserReservationsViewModel> userReservations = new List<UserReservationsViewModel>();
            UserReservationsViewModel r1 = new UserReservationsViewModel() { Author = "Adam Freeman", BookId = 1, ReservationId = 1, Title = "ASP.NET MVC4 Zaawansowane programowanie", DateOfReceipt = new DateTime(2016, 02, 11, 15, 24, 17) };
            UserReservationsViewModel r2 = new UserReservationsViewModel() { Author = "Ian Griffiths", BookId = 2, ReservationId = 2, Title = "C# 5.0 Programowanie", DateOfReceipt = new DateTime(2016, 03, 16, 14, 08, 36) };
            UserReservationsViewModel r3 = new UserReservationsViewModel() { Author = "Jarosław Cisek", BookId = 3, ReservationId = 3, Title = "Tworzenie nowoczesnych aplikacji graficznych w WPF", DateOfReceipt = new DateTime(2016, 07, 30, 11, 44, 11) };
            UserReservationsViewModel r4 = new UserReservationsViewModel() { Author = "Sławomir Orłowski", BookId = 4, ReservationId = 4, Title = "C# Tworzenie aplikacji sieciowych. Gotowe projekty", DateOfReceipt = new DateTime(2016, 11, 21, 22, 35, 47) };
            userReservations.Add(r1);
            userReservations.Add(r2);
            userReservations.Add(r3);
            userReservations.Add(r4);

            mockMapper.Setup(m => m.Map<IEnumerable<UserReservationsViewModel>>(It.IsAny<IEnumerable<Reservation>>())).Returns(userReservations);

            ReservationController controller = new ReservationController(mockReservationService.Object, mockMapper.Object, mockBookService.Object, mockRentService.Object,mockUserService.Object);

            ViewResult vr = controller.IndexUserReservations(null) as ViewResult;

            IEnumerable<UserReservationsViewModel> reservations = (IEnumerable<UserReservationsViewModel>)vr.Model;

            CollectionAssert.AreEqual(userReservations, reservations.ToList());
            Assert.AreEqual(4, reservations.Count());
        }

        [TestMethod]
        public void IndexUserReservations_return_model_state_error_if_bookId_is_not_equal_to_null()
        {
            mockRentService.Setup(m => m.GetAllRents());

            List<UserReservationsViewModel> userReservations = new List<UserReservationsViewModel>();

            mockMapper.Setup(m => m.Map<IEnumerable<UserReservationsViewModel>>(It.IsAny<IEnumerable<Reservation>>())).Returns(userReservations);

            ReservationController controller = new ReservationController(mockReservationService.Object, mockMapper.Object, mockBookService.Object, mockRentService.Object, mockUserService.Object);
            controller.ModelState.Clear();
            int bookId = 1;
            ViewResult vr = controller.IndexUserReservations(bookId) as ViewResult;

            Assert.IsTrue(vr.ViewData.ModelState.Count == 1);
        }

        #endregion


        #region AddReservation/ConfirmReservation

        [TestMethod]
        public void AddReservation_return_HttpNotFoun_if_book_is_not_found()
        {
            Book book = null;
            mockBookService.Setup(m => m.FindBook(It.IsAny<int>())).Returns(book);

            ReservationController controller = new ReservationController(mockReservationService.Object, mockMapper.Object, mockBookService.Object, mockRentService.Object, mockUserService.Object);
            var result = controller.AddReservation(It.IsAny<int>()) as HttpNotFoundResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }


        [TestMethod]
        public void AddReservation_redirect_to_BookIsNotAvailable_action_if_book_quantity_is_equal_to_zero()
        {
            Book book = new Book();
            mockBookService.Setup(m => m.FindBook(It.IsAny<int>())).Returns(book);

            ReservationController controller = new ReservationController(mockReservationService.Object, mockMapper.Object, mockBookService.Object, mockRentService.Object, mockUserService.Object);
            var result = controller.AddReservation(10) as RedirectToRouteResult;
            
            Assert.IsNotNull(result);
            Assert.AreEqual("BookIsNotAvailable", result.RouteValues["action"], "Invalid action name");
            Assert.AreEqual("Book", result.RouteValues["controller"], "Invalid controller name");
            Assert.AreEqual(10, result.RouteValues["bookId"], "Invalid bookId value");
        }

        [TestMethod]
        public void ConfirmReservation_redirect_to_BookIsNotAvailable_action_if_book_is_not_available()
        {
            mockUserService.Setup(m => m.CurrentUserId());
            mockBookService.Setup(m => m.BookIsAvailable(It.IsAny<int>())).Returns(false);
            mockUserService.Setup(m => m.UserAddressExists(It.IsAny<int>())).Returns(true);
            ReservationController controller = new ReservationController(mockReservationService.Object, mockMapper.Object, mockBookService.Object, mockRentService.Object, mockUserService.Object);
            var result = controller.ConfirmReservation(10,string.Empty) as RedirectToRouteResult;
            
            Assert.IsNotNull(result);
            Assert.AreEqual("BookIsNotAvailable", result.RouteValues["action"], "Invalid action name");
            Assert.AreEqual("Book", result.RouteValues["controller"], "Invalid controller name");
            Assert.AreEqual(10, result.RouteValues["bookId"], "Invalid bookId value");
        }


        [TestMethod]
        public void ConfirmReservation_redirect_to_IndexUserReservations_if_reservation_exists()
        {
            mockUserService.Setup(m => m.CurrentUserId());
            mockBookService.Setup(m => m.BookIsAvailable(It.IsAny<int>())).Returns(true);
            mockReservationService.Setup(m => m.ReservationExists(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            mockUserService.Setup(m => m.UserAddressExists(It.IsAny<int>())).Returns(true);

            ReservationController controller = new ReservationController(mockReservationService.Object, mockMapper.Object, mockBookService.Object, mockRentService.Object, mockUserService.Object);
            var result = controller.ConfirmReservation(10, string.Empty) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("IndexUserReservations", result.RouteValues["action"], "Invalid action name");
            Assert.AreEqual(10, result.RouteValues["bookId"], "Invalid bookId value");
        }


        [TestMethod]
        public void ConfirmReservation_redirect_to_IndexUserRents_action_if_rent_exists()
        {
            mockUserService.Setup(m => m.CurrentUserId());
            mockBookService.Setup(m => m.BookIsAvailable(It.IsAny<int>())).Returns(true);
            mockReservationService.Setup(m => m.ReservationExists(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            mockRentService.Setup(m => m.RentExists(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            mockUserService.Setup(m => m.UserAddressExists(It.IsAny<int>())).Returns(true);

            ReservationController controller = new ReservationController(mockReservationService.Object, mockMapper.Object, mockBookService.Object, mockRentService.Object, mockUserService.Object);
            var result = controller.ConfirmReservation(10, string.Empty) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("IndexUserRents", result.RouteValues["action"], "Invalid action name");
            Assert.AreEqual("Rent", result.RouteValues["controller"], "Invalid controller name");
            Assert.AreEqual(10, result.RouteValues["bookId"], "Invalid bookId value");
        }


        [TestMethod]
        public void ConfirmReservation_redirect_to_AddAddress_action_if_user_address_not_exists()
        {
            mockUserService.Setup(m => m.CurrentUserId());
            mockBookService.Setup(m => m.BookIsAvailable(It.IsAny<int>())).Returns(true);
            mockReservationService.Setup(m => m.ReservationExists(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            mockRentService.Setup(m => m.RentExists(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            mockUserService.Setup(m => m.UserAddressExists(It.IsAny<int>())).Returns(false);

            ReservationController controller = new ReservationController(mockReservationService.Object, mockMapper.Object, mockBookService.Object, mockRentService.Object, mockUserService.Object);
            var result = controller.ConfirmReservation(10, string.Empty) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("AddAddress", result.RouteValues["action"], "Invalid action name");
            Assert.AreEqual("Account", result.RouteValues["controller"], "Invalid controller name");
        }


        [TestMethod]
        public void ConfirmReservation_can_add_new_reservation_and_redirect_to_correct_url()
        {
            string url = "http://localhost:53883/Book?searchBook=mvc&amp;searchAuthor=";
            mockUserService.Setup(m => m.CurrentUserId());
            mockBookService.Setup(m => m.BookIsAvailable(It.IsAny<int>())).Returns(true);
            mockReservationService.Setup(m => m.ReservationExists(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            mockRentService.Setup(m => m.RentExists(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            mockUserService.Setup(m => m.UserAddressExists(It.IsAny<int>())).Returns(true);

            ReservationController controller = new ReservationController(mockReservationService.Object, mockMapper.Object, mockBookService.Object, mockRentService.Object, mockUserService.Object);
            var result = controller.ConfirmReservation(10, url) as RedirectResult;
            //mock websecurity
            Assert.IsNotNull(result);
            Assert.AreEqual(url, result.Url, "Invalid url");
            mockReservationService.Verify(m=>m.AddReservation(It.IsAny<Reservation>()),Times.Once);
        }

        #endregion


        #region DeleteReservation/DeleteReservationConfirm

        [TestMethod]
        public void DeleteReservation_return_HttpNotFound_if_reservation_is_not_found()
        {
            Reservation reservation = null;
            mockReservationService.Setup(m => m.FindReservation(It.IsAny<int>())).Returns(reservation);

            ReservationController controller = new ReservationController(mockReservationService.Object, mockMapper.Object, mockBookService.Object, mockRentService.Object, mockUserService.Object);
            var result = controller.DeleteReservation(It.IsAny<int>()) as HttpNotFoundResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public void DeleteReservationConfirm_can_delete_reservation_and_redirect_to_IndexUserReservations_action()
        {
            Reservation reservation = new Reservation { ReservationId = 1, BookId = 1, UserId = 1, DateOfReceipt = new DateTime(2016, 3, 15, 15, 26, 14), ReservationDate = new DateTime(2016, 3, 14, 15, 26, 14) };
            mockReservationService.Setup(m => m.FindReservation(It.IsAny<int>())).Returns(reservation);
            mockReservationService.Setup(m => m.DeleteReservation(It.IsAny<int>()));

            ReservationController controller = new ReservationController(mockReservationService.Object, mockMapper.Object, mockBookService.Object, mockRentService.Object, mockUserService.Object);

            var result = controller.DeleteReservationConfirm(It.IsAny<int>()) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("IndexUserReservations", result.RouteValues["action"], "Invalid action name");
            mockReservationService.Verify(m => m.DeleteReservation(It.IsAny<int>()), Times.Once);
        }

        #endregion
    }
}
