//using System;
//using System.Text;
//using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using BooksLibrary.Business.Models;
//using BooksLibrary.Business.Service;
//using Moq;
//using System.Linq;
//using BooksLibrary.Controllers;
//using AutoMapper;
//using BooksLibrary.Models;
//using System.Web.Mvc;
//using System.Linq.Expressions;
//using System.Collections;
//using PagedList;
//using BooksLibrary.Models.BookViewModels;
//using System.Web;
//using System.Web.Routing;

//namespace BooksLibrary.Tests.Controllers
//{
//    [TestClass]
//    public class BookControllerTest
//    {
//        Mock<IBookService> mock = null;
//        Mock<IMapper> mockMapper = null;

//        public BookControllerTest()
//        {
//            mock = new Mock<IBookService>();
//            mockMapper = new Mock<IMapper>();
//        }


//        #region Index

//        [TestMethod]
//        public void Index_return_list_of_books()
//        {
//            mock.Setup(m => m.GetAllBooks());

//            List<IndexAllBooksViewModel> indexBooks = new List<IndexAllBooksViewModel>();
//            IndexAllBooksViewModel bk1 = new IndexAllBooksViewModel() { BookId = 1, Title = "ASP.NET MVC4 Zaawansowane programowanie", Author = "Adam Freeman", Category = "Programowanie", Quantity = 5 };
//            IndexAllBooksViewModel bk2 = new IndexAllBooksViewModel() { BookId = 2, Title = "C# 5.0 Programowanie", Author = "Ian Griffiths", Category = "Programowanie4", Quantity = 2 };
//            IndexAllBooksViewModel bk3 = new IndexAllBooksViewModel { BookId = 3, Title = "Tworzenie nowoczesnych aplikacji graficznych w WPF", Author = "Jarosław Cisek", Category = "Programowanie", Quantity = 7 };
//            IndexAllBooksViewModel bk4 = new IndexAllBooksViewModel { BookId = 4, Title = "C# Tworzenie aplikacji sieciowych. Gotowe projekty", Author = "Sławomir Orłowski", Category = "Programowanie", Quantity = 1 };
//            indexBooks.Add(bk1);
//            indexBooks.Add(bk2);
//            indexBooks.Add(bk3);
//            indexBooks.Add(bk4);

//            mockMapper.Setup(m => m.Map<IEnumerable<IndexAllBooksViewModel>>(It.IsAny<IEnumerable<Book>>())).Returns(indexBooks);

//            BookController controller = new BookController(mock.Object, mockMapper.Object);

//            ViewResult vr = controller.Index(null, "", "", "", "") as ViewResult;
//            IEnumerable<IndexAllBooksViewModel> allBooks = (IEnumerable<IndexAllBooksViewModel>)vr.Model;

//            CollectionAssert.AreEqual(indexBooks, allBooks.ToList());
//            Assert.IsTrue(allBooks.Count() == 4);
//        }

//        #endregion


//        #region AddBook

//        [TestMethod]
//        public void AddBook_can_add_new_book_and_redirect_to_correct_action_if_model_state_is_valid()
//        {
//            mock.Setup(m => m.AddBook(It.IsAny<Book>()));

//            BookController controller = new BookController(mock.Object, mockMapper.Object);

//            var route = controller.AddBook(It.IsAny<BookViewModel>()) as RedirectToRouteResult;

//            mock.Verify(m => m.AddBook(It.IsAny<Book>()), Times.Once);
//            Assert.AreEqual("IndexBookManagement", route.RouteValues["action"], "Invalid action name");
//        }

//        #endregion


//        #region EditBook

//        [TestMethod]
//        public void EditBook_return_HttpNotFound_if_book_is_not_found()
//        {
//            Book book = null;
//            mock.Setup(m => m.FindBook(It.IsAny<int>())).Returns(book);

//            BookController controller = new BookController(mock.Object, mockMapper.Object);

//            var result = controller.EditBook(10) as HttpNotFoundResult;

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//        }

//        [TestMethod]
//        public void EditBook_can_edit_book_and_redirect_to_correct_url_if_model_state_is_valid()
//        {
//            string url = "http://localhost:53883/Book/IndexBookManagement?page=3";
//            mock.Setup(m => m.UpdateBook(It.IsAny<Book>()));
//            BookController controller = new BookController(mock.Object, mockMapper.Object);

//            var route = controller.EditBook(It.IsAny<BookViewModel>(), url) as RedirectResult;

//            mock.Verify(m => m.UpdateBook(It.IsAny<Book>()), Times.Once);
//            Assert.AreEqual(url, route.Url, "Invalid url");
//        }

//        [TestMethod]
//        public void EditBook_return_model_state_error_if_model_state_is_not_valid()
//        {
//            BookController controller = new BookController(mock.Object, mockMapper.Object);
//            controller.ModelState.Clear();
//            controller.ModelState.AddModelError("", "Error");
//            var result = controller.EditBook(It.IsAny<BookViewModel>(), It.IsAny<string>()) as ViewResult;

//            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
//        }

//        #endregion


//        #region DeleteBook

//        [TestMethod]
//        public void DeleteBook_return_HttpNotFound_if_book_is_not_found()
//        {
//            Book book = null;
//            mock.Setup(m => m.FindBook(It.IsAny<int>())).Returns(book);

//            BookController controller = new BookController(mock.Object, mockMapper.Object);

//            var result = controller.DeleteBook(10) as HttpNotFoundResult;

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//        }

//        [TestMethod]
//        public void DeleteBookConfirm_can_delete_book_and_redirect_to_correct_action()
//        {
//            mock.Setup(m => m.DeleteBook(It.IsAny<int>()));

//            BookController controller = new BookController(mock.Object, mockMapper.Object);

//            var route = controller.DeleteBookConfirm(It.IsAny<int>(), It.IsAny<string>()) as RedirectToRouteResult;

//            mock.Verify(m => m.DeleteBook(It.IsAny<int>()), Times.Once);
//            Assert.IsNotNull(route);
//            Assert.AreEqual("IndexBookManagement", route.RouteValues["action"], "Invalid action name");


//        }

//        #endregion


//        #region Details

//        [TestMethod]
//        public void Details_return_HttpNotFound_if_book_is_not_found()
//        {
//            Book book = null;
//            mock.Setup(m => m.FindBook(It.IsAny<int>())).Returns(book);

//            BookController controller = new BookController(mock.Object, mockMapper.Object);

//            var result = controller.Details(It.IsAny<int>()) as HttpNotFoundResult;

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//        }

//        #endregion


//        #region BookIsNotAvailable

//        [TestMethod]
//        public void BookIsNotAvailable_return_HttpNotFound_if_book_is_not_found()
//        {
//            Book book = null;
//            mock.Setup(m => m.FindBook(It.IsAny<int>())).Returns(book);

//            BookController controller = new BookController(mock.Object, mockMapper.Object);

//            var result = controller.BookIsNotAvailable(It.IsAny<int>()) as HttpNotFoundResult;

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//        }

//        #endregion
//    }
//}
