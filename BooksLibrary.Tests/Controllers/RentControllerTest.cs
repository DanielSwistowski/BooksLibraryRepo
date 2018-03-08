using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BooksLibrary.Business.Service;
using AutoMapper;
using System.Collections.Generic;
using BooksLibrary.Models.RentViewModel;
using BooksLibrary.Business.Models;
using BooksLibrary.Controllers;
using System.Web.Mvc;
using System.Linq;
using BooksLibrary.Business.SecurityServices;

namespace BooksLibrary.Tests.Controllers
{
    [TestClass]
    public class RentControllerTest
    {
        private Mock<IRentService> mockRent = null;
        private Mock<IMapper> mockMapper = null;
        private Mock<IUserService> mockUserService = null;

        public RentControllerTest()
        {
            mockRent = new Mock<IRentService>();
            mockMapper = new Mock<IMapper>();
            mockUserService = new Mock<IUserService>();
        }

        [TestMethod]
        public void IndexUserRents_renturn_all_user_rents()
        {
            mockRent.Setup(m => m.GetAllRents());
            mockUserService.Setup(m => m.CurrentUserId());
            List<UserRentsViewModel> rents = new List<UserRentsViewModel>();
            UserRentsViewModel r1 = new UserRentsViewModel() { Author = "Adam Freeman", BookId = 1, Title = "ASP.NET MVC4 Zaawansowane programowanie", ReturnDate = new DateTime(2016, 02, 11, 15, 24, 17), RentDate = new DateTime(2016, 02, 11, 15, 24, 17) };
            UserRentsViewModel r2 = new UserRentsViewModel() { Author = "Ian Griffiths", BookId = 2, Title = "C# 5.0 Programowanie", ReturnDate = new DateTime(2016, 03, 16, 14, 08, 36), RentDate = new DateTime(2016, 02, 11, 15, 24, 17) };
            UserRentsViewModel r3 = new UserRentsViewModel() { Author = "Jarosław Cisek", BookId = 3, Title = "Tworzenie nowoczesnych aplikacji graficznych w WPF", ReturnDate = new DateTime(2016, 07, 30, 11, 44, 11), RentDate = new DateTime(2016, 02, 11, 15, 24, 17) };
            UserRentsViewModel r4 = new UserRentsViewModel() { Author = "Sławomir Orłowski", BookId = 4, Title = "C# Tworzenie aplikacji sieciowych. Gotowe projekty", ReturnDate = new DateTime(2016, 11, 21, 22, 35, 47), RentDate = new DateTime(2016, 02, 11, 15, 24, 17) };
            rents.Add(r1);
            rents.Add(r2);
            rents.Add(r3);
            rents.Add(r4);

            mockMapper.Setup(m => m.Map<IEnumerable<UserRentsViewModel>>(It.IsAny<IEnumerable<Rent>>())).Returns(rents);

            RentController controller = new RentController(mockRent.Object, mockMapper.Object, mockUserService.Object);

            var result = controller.IndexUserRents(null) as ViewResult;

            IEnumerable<UserRentsViewModel> userRents = (IEnumerable<UserRentsViewModel>)result.Model;

            CollectionAssert.AreEqual(rents, userRents.ToList());
        }


        [TestMethod]
        public void IndexUserRents_return_model_state_error_if_bookId_is_not_equal_to_null()
        {
            mockRent.Setup(m => m.GetAllRents());

            List<UserRentsViewModel> rents = new List<UserRentsViewModel>();

            mockMapper.Setup(m => m.Map<IEnumerable<UserRentsViewModel>>(It.IsAny<IEnumerable<Rent>>())).Returns(rents);

            RentController controller = new RentController(mockRent.Object, mockMapper.Object, mockUserService.Object);

            controller.ModelState.Clear();

            int bookId = 1;
            var result = controller.IndexUserRents(bookId) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1);
        }
    }
}
