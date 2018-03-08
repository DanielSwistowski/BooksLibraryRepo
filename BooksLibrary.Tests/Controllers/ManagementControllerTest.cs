using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BooksLibrary.Business.Service;
using Moq;
using AutoMapper;
using System.Collections.Generic;
using BooksLibrary.Models.ReservationViewModels;
using BooksLibrary.Business.Models;
using BooksLibrary.Controllers;
using System.Web.Mvc;
using System.Linq;
using BooksLibrary.Models.ManagementViewModels;

namespace BooksLibrary.Tests.Controllers
{
    [TestClass]
    public class ManagementControllerTest
    {
        Mock<IBookService> mockBookService = null;
        Mock<IReservationService> mockReservationService = null;
        Mock<IRentService> mockRentService = null;
        Mock<IManagementService> mockManagementService = null;
        Mock<IMapper> mockMapper = null;

        public ManagementControllerTest()
        {
            mockBookService = new Mock<IBookService>();
            mockReservationService = new Mock<IReservationService>();
            mockRentService = new Mock<IRentService>();
            mockManagementService = new Mock<IManagementService>();
            mockMapper = new Mock<IMapper>();
        }

        [TestMethod]
        public void ReservationsManagement_return_all_reservations()
        {
            List<ReservationsManagementViewModel> reservations = new List<ReservationsManagementViewModel>();
            ReservationsManagementViewModel r1 = new ReservationsManagementViewModel() { UserFullName="Daniel Świstowski", Author = "Adam Freeman", ReservationID = 1, Title = "ASP.NET MVC4 Zaawansowane programowanie", ReservationDate = new DateTime(2016, 02, 11, 15, 24, 17) };
            ReservationsManagementViewModel r2 = new ReservationsManagementViewModel() { UserFullName="Jan Kowalski", Author = "Ian Griffiths", ReservationID = 2, Title = "C# 5.0 Programowanie", ReservationDate = new DateTime(2016, 03, 16, 14, 08, 36) };
            ReservationsManagementViewModel r3 = new ReservationsManagementViewModel() { UserFullName="Tomasz Nowak", Author = "Jarosław Cisek", ReservationID = 3, Title = "Tworzenie nowoczesnych aplikacji graficznych w WPF", ReservationDate = new DateTime(2016, 07, 30, 11, 44, 11) };
            ReservationsManagementViewModel r4 = new ReservationsManagementViewModel() { UserFullName="Jan Kowalski", Author = "Sławomir Orłowski", ReservationID = 4, Title = "C# Tworzenie aplikacji sieciowych. Gotowe projekty", ReservationDate = new DateTime(2016, 11, 21, 22, 35, 47) };
            reservations.Add(r1);
            reservations.Add(r2);
            reservations.Add(r3);
            reservations.Add(r4);

            mockReservationService.Setup(m => m.GetAllReservations());
            mockMapper.Setup(m => m.Map<IEnumerable<ReservationsManagementViewModel>>(It.IsAny<IEnumerable<Reservation>>())).Returns(reservations);

            ManagementController controller = new ManagementController(mockReservationService.Object, mockManagementService.Object, mockBookService.Object, mockRentService.Object, mockMapper.Object);

            var result = controller.ReservationsManagement(null,string.Empty,string.Empty,string.Empty) as ViewResult;

            IEnumerable<ReservationsManagementViewModel> res = (IEnumerable<ReservationsManagementViewModel>)result.Model;

            CollectionAssert.AreEqual(reservations, res.ToList());
        }

        [TestMethod]
        public void RentsManagement_return_all_rents()
        {
            List<RentsManagementViewModel> rents = new List<RentsManagementViewModel>();
            RentsManagementViewModel r1 = new RentsManagementViewModel() { UserFullName = "Daniel Świstowski", Author = "Adam Freeman", RentId = 1, BookId = 1, Title = "ASP.NET MVC4 Zaawansowane programowanie", ReturnDate = new DateTime(2016, 02, 11, 15, 24, 17) };
            RentsManagementViewModel r2 = new RentsManagementViewModel() { UserFullName = "Jan Kowalski", Author = "Ian Griffiths", RentId = 2, BookId = 2, Title = "C# 5.0 Programowanie", ReturnDate = new DateTime(2016, 03, 16, 14, 08, 36) };
            RentsManagementViewModel r3 = new RentsManagementViewModel() { UserFullName = "Tomasz Nowak", Author = "Jarosław Cisek", RentId = 3, BookId = 3, Title = "Tworzenie nowoczesnych aplikacji graficznych w WPF", ReturnDate = new DateTime(2016, 07, 30, 11, 44, 11) };
            RentsManagementViewModel r4 = new RentsManagementViewModel() { UserFullName = "Jan Kowalski", Author = "Sławomir Orłowski", RentId = 4, BookId = 4, Title = "C# Tworzenie aplikacji sieciowych. Gotowe projekty", ReturnDate = new DateTime(2016, 11, 21, 22, 35, 47) };
            rents.Add(r1);
            rents.Add(r2);
            rents.Add(r3);
            rents.Add(r4);

            mockRentService.Setup(m => m.GetAllRents());
            mockMapper.Setup(m => m.Map<IEnumerable<RentsManagementViewModel>>(It.IsAny<IEnumerable<Rent>>())).Returns(rents);

            ManagementController controller = new ManagementController(mockReservationService.Object, mockManagementService.Object, mockBookService.Object, mockRentService.Object, mockMapper.Object);

            var result = controller.RentsManagement(null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) as ViewResult;

            IEnumerable<RentsManagementViewModel> rnt = (IEnumerable<RentsManagementViewModel>)result.Model;

            CollectionAssert.AreEqual(rents, rnt.ToList());
        }


        [TestMethod]
        public void ReleaseBook_return_HttpNotFound_if_reservation_is_not_found()
        {
            Reservation reservation = null;
            mockReservationService.Setup(m => m.FindReservation(It.IsAny<int>())).Returns(reservation);
            ManagementController controller = new ManagementController(mockReservationService.Object, mockManagementService.Object, mockBookService.Object, mockRentService.Object, mockMapper.Object);
            
            var result = controller.ReleaseBook(It.IsAny<int>()) as HttpNotFoundResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }


        [TestMethod]
        public void ReleaseBook_can_release_book_and_redirect_to_correct_url()
        {
            string url = "http://localhost:53883/Management/ReservationsManagement?searchUserFirstName=adam&searchUserLastName=freeman&searchBook=";
            mockManagementService.Setup(m => m.ReleaseBook(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            ManagementController controller = new ManagementController(mockReservationService.Object, mockManagementService.Object, mockBookService.Object, mockRentService.Object, mockMapper.Object);

            ReleaseBookViewModel model = new ReleaseBookViewModel();
            var result = controller.ReleaseBook(model,url) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(url, result.Url, "Invalid url");
            mockManagementService.Verify(m => m.ReleaseBook(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }


        [TestMethod]
        public void ReturnBook_return_HttpNotFound_if_rent_is_not_found()
        {
            Rent rent = null;
            mockRentService.Setup(m => m.FindRent(It.IsAny<int>())).Returns(rent);

            ManagementController controller = new ManagementController(mockReservationService.Object, mockManagementService.Object, mockBookService.Object, mockRentService.Object, mockMapper.Object);

            var result = controller.ReturnBook(It.IsAny<int>()) as HttpNotFoundResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }


        [TestMethod]
        public void ReturnBookConfirm_can_return_book_and_redirect_to_correct_route()
        {
            string returnUrl = "http://localhost:53883/Management/RentsManagement?searchUserFirstName=Daniel&searchUserLastName=&searchBook=";
            mockManagementService.Setup(m => m.ReturnBook(It.IsAny<int>()));
            mockManagementService.Setup(m => m.ShouldEnableUserAccount(It.IsAny<int>())).Returns(false);
            ManagementController controller = new ManagementController(mockReservationService.Object, mockManagementService.Object, mockBookService.Object, mockRentService.Object, mockMapper.Object);

            var result = controller.ReturnBookConfirm(It.IsAny<int>(),It.IsAny<int>(), returnUrl) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(returnUrl, result.Url, "Invalid url");
            mockManagementService.Verify(m => m.ReturnBook(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void ReturnBookConfirm_can_return_book_and_redirect_to_EnableUserAccount_action()
        {
            string returnUrl = null;
            mockManagementService.Setup(m => m.ReturnBook(It.IsAny<int>()));
            mockManagementService.Setup(m => m.ShouldEnableUserAccount(It.IsAny<int>())).Returns(true);
            ManagementController controller = new ManagementController(mockReservationService.Object, mockManagementService.Object, mockBookService.Object, mockRentService.Object, mockMapper.Object);

            var result = controller.ReturnBookConfirm(It.IsAny<int>(), 1, returnUrl) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("EnableUserAccount", result.RouteValues["action"], "Invalid url");
            Assert.AreEqual("Account", result.RouteValues["Controller"], "Invalid controller name");
            Assert.AreEqual(1, result.RouteValues["userId"], "Invalid user id");
            mockManagementService.Verify(m => m.ReturnBook(It.IsAny<int>()), Times.Once);
            mockManagementService.Verify(m => m.ShouldEnableUserAccount(It.IsAny<int>()), Times.Once);
        }
    }
}