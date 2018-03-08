using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BooksLibrary.Business.UnitOfWork;
using BooksLibrary.Business.Models;
using BooksLibrary.Business.Repository;
using BooksLibrary.Business.Service;

namespace BookLibrary.Business.Tests.Services
{
    /// <summary>
    /// Summary description for BookServiceTest
    /// </summary>
    [TestClass]
    public class BookServiceTest
    {
        private Mock<IUnitOfWork> mockUoW = null;
        private Mock<IRepository<Book>> mockBookRepo = null;
        private List<Book> booksList = null;
        private Mock<IRepository<Rent>> mockRentRepo = null;
        private Mock<IRepository<Reservation>> mockReservationRepo = null;

        public BookServiceTest()
        {
            mockUoW = new Mock<IUnitOfWork>();
            mockBookRepo = new Mock<IRepository<Book>>();
            mockRentRepo = new Mock<IRepository<Rent>>();
            mockReservationRepo = new Mock<IRepository<Reservation>>();

            booksList = new List<Book>
            {
                new Book { BookId=1, Title = "ASP.NET MVC4 Zaawansowane programowanie", Author = "Adam Freeman", ISBN = "JLRKAQAOSG3L", ReleaseDate = "2013", PublishingHouse = "Helion", Quantity = 5 },
                new Book { BookId=2, Title = "C# 5.0 Programowanie", Author = "Ian Griffiths", ISBN = "P2YJ31IRJRFH", ReleaseDate = "2014", PublishingHouse = "Helion", Quantity = 2 },
                new Book { BookId=3, Title = "Tworzenie nowoczesnych aplikacji graficznych w WPF", Author = "Jarosław Cisek", ISBN = "I7RLP0Y9TPGB", ReleaseDate = "2012", PublishingHouse = "Helion", Quantity = 7 },
                new Book { BookId=4, Title = "C# Tworzenie aplikacji sieciowych. Gotowe projekty", Author = "Sławomir Orłowski", ISBN = "WC42ROAH5UU2", ReleaseDate = "2010", PublishingHouse = "Helion", Quantity = 1 }
            };
        }

        #region GetAllBooks

        [TestMethod]
        public void can_get_all_books()
        {
            mockBookRepo.Setup(m => m.Get()).Returns(booksList.AsQueryable());
            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);

            BookService bookService = new BookService(mockUoW.Object);

            var result = bookService.GetAllBooks().ToList();

            CollectionAssert.AreEqual(booksList, result);
        }

        [TestMethod]
        public void get_all_books_can_filter_data()
        {
            mockBookRepo.Setup(m => m.Get()).Returns(booksList.AsQueryable());
            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);

            BookService bookService = new BookService(mockUoW.Object);

            var result = bookService.GetAllBooks(b => b.Title.StartsWith("C")).ToList();

            var book = booksList.Where(b => b.Title.StartsWith("C")).ToList();

            Assert.AreEqual(result.Count(), 2);
            CollectionAssert.AreEqual(result, book);
        }

        [TestMethod]
        public void get_all_books_can_sort_data()
        {
            mockBookRepo.Setup(m => m.Get()).Returns(booksList.AsQueryable());
            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);

            BookService bookService = new BookService(mockUoW.Object);

            int totalCount;
            var result = bookService.GetAllBooks(null, null, b => b.OrderBy(o => o.Quantity), out totalCount, null, null).ToList();

            Assert.AreEqual(result[0].Quantity, 1);
            Assert.AreEqual(result[3].Quantity, 7);
        }

        [TestMethod]
        public void can_get_all_books_with_filtering_sorting_and_paging()
        {
            mockBookRepo.Setup(m => m.Get()).Returns(booksList.AsQueryable());
            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);

            BookService bookService = new BookService(mockUoW.Object);

            int totalCount;
            int pageSize = 1;
            int pageNumber = 2;
            var result = bookService.GetAllBooks(b => b.Title.StartsWith("C"), null, b => b.OrderBy(o => o.Quantity), out totalCount, pageNumber, pageSize).ToList();

            Assert.AreEqual(result.Count(), 1);
            Assert.AreEqual(result[0].Quantity, 2);
            Assert.AreEqual(totalCount, 2);
            Assert.AreEqual(result[0].Title, "C# 5.0 Programowanie");
        }

        #endregion


        #region FindBook

        [TestMethod]
        public void can_find_book_by_id()
        {
            mockBookRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns((int i) => booksList.Single(b => b.BookId == i));
            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);
            BookService bookService = new BookService(mockUoW.Object);

            var result = bookService.FindBook(2);
            var expected = booksList.Single(b => b.BookId == 2);

            Assert.AreEqual(expected, result);
        }

        #endregion


        #region AddBook

        [TestMethod]
        public void can_add_new_book()
        {
            mockBookRepo.Setup(m => m.Add(It.IsAny<Book>())).Callback((Book book) => booksList.Add(book));
            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);

            BookService bookService = new BookService(mockUoW.Object);
            Book bookToAdd = new Book();
            bookService.AddBook(bookToAdd);

            Assert.AreEqual(5, booksList.Count);
            Assert.IsTrue(booksList.Contains(bookToAdd));
            mockBookRepo.Verify(m => m.Add(bookToAdd), Times.Once);
        }

        #endregion


        #region UpdateBook

        [TestMethod]
        public void can_update_book()
        {
            mockBookRepo.Setup(m => m.Update(It.IsAny<Book>())).Callback((Book book) =>
                {
                    var bk = booksList.Where(b => b.BookId == 1).Single();
                    booksList.Remove(bk);
                    booksList.Add(book);
                });

            mockBookRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(new Book { BookId = 1, Title = "ASP.NET MVC4 Zaawansowane programowanie", Author = "Adam Freeman", ISBN = "JLRKAQAOSG3L", ReleaseDate = "2013", PublishingHouse = "Helion", Quantity = 5 });

            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);

            BookService bookService = new BookService(mockUoW.Object);
            Book updatedBook = new Book() { BookId = 1, Title = "ASP.NET MVC4", Author = "Adam Freeman", ISBN = "JLRKAQAOSG3L", ReleaseDate = "2013", PublishingHouse = "Helion", Quantity = 25 };
            bookService.UpdateBook(updatedBook);

            Assert.AreEqual(4, booksList.Count);
            Assert.IsTrue(booksList.Contains(updatedBook));
            Assert.AreEqual(25, booksList.Where(b => b.BookId == 1).Single().Quantity);
            mockBookRepo.Verify(m => m.Update(updatedBook), Times.Once);
        }

        #endregion


        #region DeleteBook

        [TestMethod]
        public void can_delete_book()
        {
            mockBookRepo.Setup(m => m.Delete(It.IsAny<Book>())).Callback((Book book) =>
                {
                    var bk = booksList.Where(b => b.BookId == 1).Single();
                    booksList.Remove(bk);
                });

            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);

            BookService bookService = new BookService(mockUoW.Object);
            bookService.DeleteBook(1);

            Assert.AreEqual(3, booksList.Count);
            Assert.AreEqual(null, booksList.Find(b => b.BookId == 1));
            mockBookRepo.Verify(m => m.Delete(It.IsAny<Book>()), Times.Once);
        }

        #endregion


        #region GetPropablyBookAvailableDate

        [TestMethod]
        public void GetPropablyBookAvailableDate_return_correct_date_if_book_exists_in_rents_and_reservations()
        {
            var expectedResult = new DateTime(2016, 10, 20, 12, 12, 12);
            List<Reservation> reservations = new List<Reservation>() { new Reservation { BookId = 1, DateOfReceipt = expectedResult } };
            List<Rent> rents = new List<Rent>() { new Rent { BookId = 1, ReturnDate = new DateTime(2016, 10, 24, 12, 12, 12) } };

            mockReservationRepo.Setup(m => m.Get()).Returns(reservations.AsQueryable());
            mockRentRepo.Setup(m => m.Get()).Returns(rents.AsQueryable());

            mockUoW.Setup(m => m.ReservationRepository).Returns(mockReservationRepo.Object);
            mockUoW.Setup(m => m.RentRepository).Returns(mockRentRepo.Object);

            BookService service = new BookService(mockUoW.Object);
            var result = service.GetPropablyBookAvailableDate(1);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetPropablyBookAvailableDate_return_correct_date_if_book_exists_only_in_rents()
        {
            var expectedResult = new DateTime(2016, 10, 24, 12, 12, 12);
            List<Reservation> reservations = new List<Reservation>() { new Reservation { BookId = 2, DateOfReceipt = new DateTime(2016, 10, 20, 12, 12, 12) } };
            List<Rent> rents = new List<Rent>() { new Rent { BookId = 1, ReturnDate = expectedResult }, new Rent { BookId = 1, ReturnDate = new DateTime(2016, 10, 30, 12, 12, 12) } };

            mockReservationRepo.Setup(m => m.Get()).Returns(reservations.AsQueryable());
            mockRentRepo.Setup(m => m.Get()).Returns(rents.AsQueryable());

            mockUoW.Setup(m => m.ReservationRepository).Returns(mockReservationRepo.Object);
            mockUoW.Setup(m => m.RentRepository).Returns(mockRentRepo.Object);

            BookService service = new BookService(mockUoW.Object);
            var result = service.GetPropablyBookAvailableDate(1);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetPropablyBookAvailableDate_return_correct_date_if_book_exists_only_in_reservations()
        {
            var expectedResult = new DateTime(2016, 10, 20, 12, 12, 12);
            List<Reservation> reservations = new List<Reservation>() { new Reservation { BookId = 1, DateOfReceipt = expectedResult }, new Reservation { BookId = 1, DateOfReceipt = new DateTime(2016, 10, 25, 12, 12, 12) } };
            List<Rent> rents = new List<Rent>() { new Rent { BookId = 2, ReturnDate = new DateTime(2016, 10, 24, 12, 12, 12) } };

            mockReservationRepo.Setup(m => m.Get()).Returns(reservations.AsQueryable());
            mockRentRepo.Setup(m => m.Get()).Returns(rents.AsQueryable());

            mockUoW.Setup(m => m.ReservationRepository).Returns(mockReservationRepo.Object);
            mockUoW.Setup(m => m.RentRepository).Returns(mockRentRepo.Object);

            BookService service = new BookService(mockUoW.Object);
            var result = service.GetPropablyBookAvailableDate(1);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetPropablyBookAvailableDate_return_default_date_if_book_not_exists_in_rents_and_reservations()
        {
            var expectedResult = default(DateTime);
            List<Reservation> reservations = new List<Reservation>() { new Reservation { BookId = 4, DateOfReceipt = new DateTime(2016, 10, 20, 12, 12, 12) } };
            List<Rent> rents = new List<Rent>() { new Rent { BookId = 2, ReturnDate = new DateTime(2016, 10, 24, 12, 12, 12) } };

            mockReservationRepo.Setup(m => m.Get()).Returns(reservations.AsQueryable());
            mockRentRepo.Setup(m => m.Get()).Returns(rents.AsQueryable());

            mockUoW.Setup(m => m.ReservationRepository).Returns(mockReservationRepo.Object);
            mockUoW.Setup(m => m.RentRepository).Returns(mockRentRepo.Object);

            BookService service = new BookService(mockUoW.Object);
            var result = service.GetPropablyBookAvailableDate(1);

            Assert.AreEqual(expectedResult, result);
        }

        #endregion


        #region BookIsAvailable

        [TestMethod]
        public void BookIsAvailable_return_true_if_book_is_available()
        {
            Book book = new Book { BookId = 1, Title = "ASP.NET MVC4 Zaawansowane programowanie", Author = "Adam Freeman", ISBN = "JLRKAQAOSG3L", ReleaseDate = "2013", PublishingHouse = "Helion", Quantity = 5 };
            mockBookRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns(book);
            mockUoW.Setup(m => m.BookRepository).Returns(mockBookRepo.Object);

            BookService service = new BookService(mockUoW.Object);
            var result = service.BookIsAvailable(1);

            Assert.IsTrue(result);
        }

        #endregion


        #region BookExistsInRents

        [TestMethod]
        public void BookExistsInRents_return_true_if_book_exists_in_rents()
        {
            List<Rent> rents = new List<Rent>() { new Rent { BookId = 1 } };

            mockRentRepo.Setup(m => m.Get()).Returns(rents.AsQueryable());
            mockUoW.Setup(m => m.RentRepository).Returns(mockRentRepo.Object);

            BookService service = new BookService(mockUoW.Object);
            var result = service.BookExistsInRents(1);

            Assert.IsTrue(result);
        }

        #endregion


        #region BookExistsInReservations

        [TestMethod]
        public void BookExistsInReservations_return_true_if_book_exists_in_reservations()
        {
            List<Reservation> reservations = new List<Reservation>() { new Reservation { BookId = 1 } };

            mockReservationRepo.Setup(m => m.Get()).Returns(reservations.AsQueryable());
            mockUoW.Setup(m => m.ReservationRepository).Returns(mockReservationRepo.Object);

            BookService service = new BookService(mockUoW.Object);
            var result = service.BookExistsInReservations(1);

            Assert.IsTrue(result);
        }

        #endregion
    }
}
