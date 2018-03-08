using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BooksLibrary.Business.UnitOfWork;
using BooksLibrary.Business.Repository;
using BooksLibrary.Business.Models;
using System.Collections.Generic;
using System.Linq;
using BooksLibrary.Business.Service;
using System.Data.Entity;

namespace BookLibrary.Business.Tests.Services
{
    [TestClass]
    public class ReservationServiceTest
    {
        private Mock<IUnitOfWork> mockUoW = null;
        private Mock<IRepository<Reservation>> mockRepo = null;

        private UserModel user1 = null;
        private UserModel user2 = null;
        private List<Reservation> reservationsList = null;
        private Book book1 = null;
        private Book book2 = null;
        private Book book3 = null;
        private Reservation reservation = null;

        public ReservationServiceTest()
        {
            mockUoW = new Mock<IUnitOfWork>();
            mockRepo = new Mock<IRepository<Reservation>>();

            user1 = new UserModel { UserId = 1, UserName = "DanielSwistowski", Gender = Gender.Male, FirstName = "Daniel", LastName = "Świstowski", Email = "danielswistowski@wp.pl", UserIsEnabled = true };
            user2 = new UserModel { UserId = 2, UserName = "JanKowalski", Gender = Gender.Male, FirstName = "Jan", LastName = "Kowalski", Email = "jankowalski@wp.pl", UserIsEnabled = true };

            book1 = new Book { BookId = 1, Title = "ASP.NET MVC4 Zaawansowane programowanie", Author = "Adam Freeman", ISBN = "JLRKAQAOSG3L", ReleaseDate = "2013", PublishingHouse = "Helion", Quantity = 5 };
            book2 = new Book { BookId = 2, Title = "C# 5.0 Programowanie", Author = "Ian Griffiths", ISBN = "P2YJ31IRJRFH", ReleaseDate = "2014", PublishingHouse = "Helion", Quantity = 2 };
            book3 = new Book { BookId = 3, Title = "Tworzenie nowoczesnych aplikacji graficznych w WPF", Author = "Jarosław Cisek", ISBN = "I7RLP0Y9TPGB", ReleaseDate = "2012", PublishingHouse = "Helion", Quantity = 7 };

            reservation = new Reservation { ReservationId = 1, BookId = 1, UserId = 1, DateOfReceipt = new DateTime(2016, 3, 15, 15, 26, 14), ReservationDate = new DateTime(2016, 3, 14, 15, 26, 14) };

            reservationsList = new List<Reservation>
            {
                reservation,
                new Reservation { ReservationId = 2, BookId = 2, UserId = 1, DateOfReceipt = new DateTime(2016, 6, 22, 11, 21, 34), ReservationDate = new DateTime(2016, 6, 21, 11, 21, 34) },
                new Reservation { ReservationId = 3, BookId = 3, UserId = 2, DateOfReceipt = new DateTime(2016, 11, 22, 21, 42, 56), ReservationDate = new DateTime(2016, 11, 22, 21, 42, 56) }
            };
        }

        #region GetAllReservations

        [TestMethod]
        public void can_get_all_reservations()
        {
            mockRepo.Setup(m => m.Get()).Returns(reservationsList.AsQueryable());
            mockUoW.Setup(m => m.ReservationRepository).Returns(mockRepo.Object);

            ReservationService service = new ReservationService(mockUoW.Object);

            var result = service.GetAllReservations().ToList();

            CollectionAssert.AreEqual(reservationsList, result);
        }

        [TestMethod]
        public void GetAllReservations_can_filter_data()
        {
            mockRepo.Setup(m => m.Get()).Returns(reservationsList.AsQueryable());
            mockUoW.Setup(m => m.ReservationRepository).Returns(mockRepo.Object);

            ReservationService service = new ReservationService(mockUoW.Object);

            var result = service.GetAllReservations(r => r.UserId == 1).ToList();

            var reservations = reservationsList.Where(r => r.UserId == 1).ToList();

            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEqual(reservations, result);
        }

        [TestMethod]
        public void GetAllReservations_can_sort_data()
        {
            mockRepo.Setup(m => m.Get()).Returns(reservationsList.AsQueryable());
            mockUoW.Setup(m => m.ReservationRepository).Returns(mockRepo.Object);

            ReservationService service = new ReservationService(mockUoW.Object);

            int totalCount;
            var result = service.GetAllReservations(null, null, r => r.OrderByDescending(o => o.ReservationDate), out totalCount, null, null).ToList();

            Assert.AreEqual(3, result[0].BookId);
            Assert.AreEqual(1, result[2].BookId);
        }

        [TestMethod]
        public void GetAllReservations_can_get_all_reservtions_with_filtering_sorting_and_paging()
        {
            mockRepo.Setup(m => m.Get()).Returns(reservationsList.AsQueryable());
            mockUoW.Setup(m => m.ReservationRepository).Returns(mockRepo.Object);

            ReservationService service = new ReservationService(mockUoW.Object);

            int totalCount;
            int pageSize = 1;
            int pageNumber = 2;
            var result = service.GetAllReservations(u => u.UserId == 1, null, r => r.OrderByDescending(o => o.ReservationDate), out totalCount, pageNumber, pageSize).ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(2, totalCount);
            Assert.AreEqual(1, result[0].BookId);
        }

        #endregion


        #region FindReservation

        [TestMethod]
        public void can_find_reservation_by_id()
        {
            mockRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns((int i) => reservationsList.Single(r => r.ReservationId == i));
            mockUoW.Setup(m => m.ReservationRepository).Returns(mockRepo.Object);
            ReservationService service = new ReservationService(mockUoW.Object);

            var result = service.FindReservation(2);
            var expected = reservationsList.Single(r => r.ReservationId == 2);

            Assert.AreEqual(expected, result);
        }

        #endregion


        #region AddReservation

        [TestMethod]
        public void can_add_reservation()
        {
            Book book = new Book();
            book.Quantity = 10;
            Mock<IRepository<Book>> mockBookRepo = new Mock<IRepository<Book>>();
            mockBookRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(book);

            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);

            mockRepo.Setup(m => m.Add(It.IsAny<Reservation>())).Callback((Reservation reservation) => reservationsList.Add(reservation));

            mockUoW.Setup(m => m.ReservationRepository).Returns(mockRepo.Object);

            ReservationService service = new ReservationService(mockUoW.Object);
            Reservation reservationToAdd = new Reservation();
            service.AddReservation(reservationToAdd);

            Assert.IsTrue(reservationsList.Contains(reservationToAdd));
            Assert.IsTrue(book.Quantity == 9);
        }

        #endregion


        #region ReservationExists

        [TestMethod]
        public void ReservationExists_return_true_if_reservation_exists()
        {
            mockRepo.Setup(m => m.Get()).Returns(reservationsList.AsQueryable());
            mockUoW.Setup(m => m.ReservationRepository).Returns(mockRepo.Object);

            ReservationService service = new ReservationService(mockUoW.Object);
            var result = service.ReservationExists(1, 1);

            Assert.AreEqual(true, result);
        }

        #endregion


        #region DeleteReservation

        [TestMethod]
        public void can_delete_reservation()
        {
            Book book = new Book();
            book.Quantity = 10;
            Mock<IRepository<Book>> mockBookRepo = new Mock<IRepository<Book>>();
            mockBookRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(book);

            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);

            mockRepo.Setup(m => m.Delete(It.IsAny<Reservation>())).Callback((Reservation reservation2) =>
                {
                    var res = reservationsList.Where(r => r.ReservationId == 1).Single();
                    reservationsList.Remove(res);
                });

            mockRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(reservation);

            mockUoW.Setup(m => m.ReservationRepository).Returns(mockRepo.Object);

            ReservationService service = new ReservationService(mockUoW.Object);
            service.DeleteReservation(1);

            Assert.AreEqual(null, reservationsList.Find(r => r.ReservationId == 1));
            Assert.IsTrue(book.Quantity == 11);
        }

        #endregion
    }
}
