using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BooksLibrary.Business.Models;
using BooksLibrary.Business.UnitOfWork;
using Moq;
using BooksLibrary.Business.Repository;
using System.Collections.Generic;
using System.Linq;
using BooksLibrary.Business.Service;

namespace BookLibrary.Business.Tests.Services
{
    [TestClass]
    public class ManagamentServiceTest
    {
        private Mock<IUnitOfWork> mockUoW = null;
        private Mock<IRepository<Reservation>> mockReservationRepo = null;
        private Mock<IRepository<Rent>> mockRentRepo = null;
        private Mock<IRepository<Book>> mockBookRepo = null;
        private Mock<IRepository<Reminder>> mockReminderRepo = null;
        private Mock<IRepository<UserModel>> mockUserRepo = null;
        private Mock<IRepository<LockAccountReason>> mockLockAccountReasonRepo = null;

        private Reservation reservation = null;
        Book book = null;
        private List<Reservation> reservationsList = null;
        private List<Rent> rentsList = null;
        private List<Book> bookList = null;

        public ManagamentServiceTest()
        {
            mockUoW = new Mock<IUnitOfWork>();
            mockReservationRepo = new Mock<IRepository<Reservation>>();
            mockRentRepo = new Mock<IRepository<Rent>>();
            mockBookRepo = new Mock<IRepository<Book>>();
            mockReminderRepo = new Mock<IRepository<Reminder>>();
            mockUserRepo = new Mock<IRepository<UserModel>>();
            mockLockAccountReasonRepo = new Mock<IRepository<LockAccountReason>>();

            reservation = new Reservation { ReservationId = 1, BookId = 1, UserId = 1, DateOfReceipt = new DateTime(2016, 3, 15, 15, 26, 14), ReservationDate = new DateTime(2016, 3, 14, 15, 26, 14) };

            reservationsList = new List<Reservation>
            {
                reservation,
                new Reservation { ReservationId = 2, BookId = 2, UserId = 1, DateOfReceipt = new DateTime(2016, 6, 22, 11, 21, 34), ReservationDate = new DateTime(2016, 6, 21, 11, 21, 34) },
                new Reservation { ReservationId = 3, BookId = 3, UserId = 2, DateOfReceipt = new DateTime(2016, 11, 22, 21, 42, 56), ReservationDate = new DateTime(2016, 11, 22, 21, 42, 56) }
            };

            rentsList = new List<Rent>
            {
                new Rent {  BookId=1, RentId=1, UserId=1, RentDate=new DateTime(2016, 3, 15, 15, 26, 14), ReturnDate=new DateTime(2016, 3, 22, 15, 26, 14) },
                new Rent {  BookId=2, RentId=2, UserId=1, RentDate=new DateTime(2016, 4, 5, 15, 26, 14), ReturnDate=new DateTime(2016, 4, 8, 15, 26, 14) },
                new Rent {  BookId=3, RentId=3, UserId=2, RentDate=new DateTime(2016, 7, 12, 15, 26, 14), ReturnDate=new DateTime(2016, 7, 26, 15, 26, 14) },
                new Rent {  BookId=4, RentId=4, UserId=2, RentDate=new DateTime(2016, 9, 21, 15, 26, 14), ReturnDate=new DateTime(2016, 9, 24, 15, 26, 14) }
            };

            bookList = new List<Book>();
            book = new Book() { BookId = 1, Title = "ASP.NET MVC4 Zaawansowane programowanie", Author = "Adam Freeman", ISBN = "JLRKAQAOSG3L", ReleaseDate = "2013", PublishingHouse = "Helion", Quantity = 5 };
            bookList.Add(book);
        }


        #region ReleaseBook

        [TestMethod]
        public void can_release_book()
        {
            mockReservationRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(reservation);
            mockReservationRepo.Setup(m => m.Delete(It.IsAny<Reservation>())).Callback((Reservation res) =>
                {
                    var reservationToDelete = reservationsList.Where(r => r.ReservationId == 1).Single();
                    reservationsList.Remove(reservationToDelete);
                });
            mockUoW.Setup(m => m.ReservationRepository).Returns(mockReservationRepo.Object);

            mockRentRepo.Setup(m => m.Add(It.IsAny<Rent>())).Callback((Rent rent) => rentsList.Add(rent));
            mockUoW.Setup(m => m.RentRepository).Returns(mockRentRepo.Object);

            ManagementService service = new ManagementService(mockUoW.Object);

            int rentTimeInDays = 5;
            int reservationId = 1;
            bool success = service.ReleaseBook(reservationId, rentTimeInDays);

            Assert.IsTrue(success);
            Assert.AreEqual(null, reservationsList.Find(r => r.ReservationId == 1));
            Assert.AreEqual(5, rentsList.Count());
            Assert.IsTrue((rentsList[4].ReturnDate - DateTime.Now).Days == 4);
        }

        #endregion


        #region ReturnBook

        [TestMethod]
        public void can_return_book()
        {
            Rent rent = new Rent();
            mockRentRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(rent);
            mockBookRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(book);
            mockBookRepo.Setup(m => m.Update(It.IsAny<Book>())).Callback((Book bk) =>
                {
                    var bk1 = bookList.Where(b => b.BookId == 1).Single();
                    bookList.Remove(bk1);
                    bookList.Add(bk);
                });
            mockRentRepo.Setup(m => m.Delete(It.IsAny<Rent>())).Callback((Rent rnt) =>
                {
                    var rentToDelete = rentsList.Where(r => r.RentId == 1).Single();
                    rentsList.Remove(rentToDelete);
                });
            Reminder reminder = null;
            mockReminderRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(reminder);

            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);
            mockUoW.Setup(m => m.RentRepository).Returns(mockRentRepo.Object);
            mockUoW.Setup(m => m.ReminderRepository).Returns(mockReminderRepo.Object);

            ManagementService service = new ManagementService(mockUoW.Object);
            service.ReturnBook(1);

            Assert.AreEqual(6, book.Quantity);
            Assert.AreEqual(null, rentsList.Find(r => r.RentId == 1));
            mockBookRepo.Verify(m => m.Update(It.IsAny<Book>()), Times.Once);
            mockRentRepo.Verify(m => m.Delete(It.IsAny<Rent>()), Times.Once);
        }

        #endregion


        #region ShouldEnableUserAccount

        [TestMethod]
        public void ShouldEnableUserAccount_return_false_if_user_is_enabled()
        {
            UserModel user = new UserModel() { UserIsEnabled = true };

            mockUserRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(user);

            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);

            ManagementService service = new ManagementService(mockUoW.Object);
            bool result = service.ShouldEnableUserAccount(It.IsAny<int>());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldEnableUserAccount_return_false_if_account_is_disiabled_by_admin()
        {
            UserModel user = new UserModel() { UserIsEnabled = false };
            LockAccountReason lockReason = new LockAccountReason() { ReturnBookDateExpired = false };
            List<LockAccountReason> reasons = new List<LockAccountReason>();
            reasons.Add(lockReason);

            mockUserRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(user);
            mockLockAccountReasonRepo.Setup(m => m.Get()).Returns(reasons.AsQueryable());

            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);
            mockUoW.Setup(m => m.LockAccountReasonRepository).Returns(mockLockAccountReasonRepo.Object);

            ManagementService service = new ManagementService(mockUoW.Object);
            bool result = service.ShouldEnableUserAccount(It.IsAny<int>());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldEnableUserAccount_return_false_if_user_not_return_all_books_with_expired_return_date()
        {
            UserModel user = new UserModel() { UserIsEnabled = false };
            LockAccountReason lockReason = new LockAccountReason() { ReturnBookDateExpired = true, UserId=1 };
            List<LockAccountReason> reasons = new List<LockAccountReason>();
            reasons.Add(lockReason);

            mockUserRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(user);
            mockLockAccountReasonRepo.Setup(m => m.Get()).Returns(reasons.AsQueryable());
            mockRentRepo.Setup(m => m.Get()).Returns(rentsList.AsQueryable());

            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);
            mockUoW.Setup(m => m.LockAccountReasonRepository).Returns(mockLockAccountReasonRepo.Object);
            mockUoW.Setup(m => m.RentRepository).Returns(mockRentRepo.Object);

            ManagementService service = new ManagementService(mockUoW.Object);
            bool result = service.ShouldEnableUserAccount(1);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldEnableUserAccount_return_true_if_user_return_all_books_with_expired_return_date()
        {
            UserModel user = new UserModel() { UserIsEnabled = false };
            LockAccountReason lockReason = new LockAccountReason() { ReturnBookDateExpired = true, UserId = 1 };
            List<LockAccountReason> reasons = new List<LockAccountReason>();
            reasons.Add(lockReason);

            List<Rent> rents = new List<Rent>();
            var date = DateTime.Now.AddDays(1);
            rents.Add(new Rent { UserId = 1, ReturnDate = date });

            mockUserRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(user);
            mockLockAccountReasonRepo.Setup(m => m.Get()).Returns(reasons.AsQueryable());
            mockRentRepo.Setup(m => m.Get()).Returns(rents.AsQueryable());

            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);
            mockUoW.Setup(m => m.LockAccountReasonRepository).Returns(mockLockAccountReasonRepo.Object);
            mockUoW.Setup(m => m.RentRepository).Returns(mockRentRepo.Object);

            ManagementService service = new ManagementService(mockUoW.Object);
            bool result = service.ShouldEnableUserAccount(1);

            Assert.IsTrue(result);
        }

        #endregion
    }
}
