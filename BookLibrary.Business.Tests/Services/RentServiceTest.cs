using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BooksLibrary.Business.Models;
using BooksLibrary.Business.UnitOfWork;
using BooksLibrary.Business.Repository;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BooksLibrary.Business.Service;

namespace BookLibrary.Business.Tests.Services
{
    [TestClass]
    public class RentServiceTest
    {
        private Mock<IUnitOfWork> mockUoW = null;
        private Mock<IRepository<Rent>> mockRepo = null;
        private List<Rent> rentsList = null;

        public RentServiceTest()
        {
            mockUoW = new Mock<IUnitOfWork>();
            mockRepo = new Mock<IRepository<Rent>>();

            rentsList = new List<Rent>
            {
                new Rent {  BookId=1, RentId=1, UserId=1, RentDate=new DateTime(2016, 3, 15, 15, 26, 14), ReturnDate=new DateTime(2016, 3, 22, 15, 26, 14) },
                new Rent {  BookId=2, RentId=2, UserId=1, RentDate=new DateTime(2016, 4, 5, 15, 26, 14), ReturnDate=new DateTime(2016, 4, 8, 15, 26, 14) },
                new Rent {  BookId=3, RentId=3, UserId=2, RentDate=new DateTime(2016, 7, 12, 15, 26, 14), ReturnDate=new DateTime(2016, 7, 26, 15, 26, 14) },
                new Rent {  BookId=4, RentId=4, UserId=2, RentDate=new DateTime(2016, 9, 21, 15, 26, 14), ReturnDate=new DateTime(2016, 9, 24, 15, 26, 14) }
            };
        }


        #region GetAllRents

        [TestMethod]
        public void can_get_all_rents()
        {
            mockRepo.Setup(m => m.Get()).Returns(rentsList.AsQueryable());
            mockUoW.Setup(m => m.RentRepository).Returns(mockRepo.Object);

            RentService service = new RentService(mockUoW.Object);

            var result = service.GetAllRents().ToList();

            CollectionAssert.AreEqual(rentsList, result);
        }

        [TestMethod]
        public void GetAllRents_can_filter_data()
        {
            mockRepo.Setup(m => m.Get()).Returns(rentsList.AsQueryable());
            mockUoW.Setup(m => m.RentRepository).Returns(mockRepo.Object);

            RentService service = new RentService(mockUoW.Object);

            var result = service.GetAllRents(u => u.UserId == 1).ToList();
            var expected = rentsList.Where(u => u.UserId == 1).ToList();

            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetAllRents_can_sort_data()
        {
            mockRepo.Setup(m => m.Get()).Returns(rentsList.AsQueryable());
            mockUoW.Setup(m => m.RentRepository).Returns(mockRepo.Object);

            RentService service = new RentService(mockUoW.Object);

            int totalCount;
            var result = service.GetAllRents(null, null, o => o.OrderByDescending(r => r.ReturnDate), out totalCount, null, null).ToList();

            Assert.AreEqual(4, result[0].BookId);
            Assert.AreEqual(1, result[3].BookId);
        }

        [TestMethod]
        public void GetAllRents_can_get_data_with_sorting_filtering_and_paging()
        {
            mockRepo.Setup(m => m.Get()).Returns(rentsList.AsQueryable());
            mockUoW.Setup(m => m.RentRepository).Returns(mockRepo.Object);

            RentService service = new RentService(mockUoW.Object);

            int totalCount;
            int pageSize = 1;
            int pageNumber = 2;
            var result = service.GetAllRents(u => u.UserId == 1, null, o => o.OrderByDescending(r => r.ReturnDate), out totalCount, pageNumber, pageSize).ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(2, totalCount);
            Assert.AreEqual(1, result[0].BookId);
        }

        #endregion


        #region FindRent

        [TestMethod]
        public void can_find_rent_by_id()
        {
            mockRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns((int i) => rentsList.Single(r => r.RentId == i));
            mockUoW.Setup(m => m.RentRepository).Returns(mockRepo.Object);

            RentService service = new RentService(mockUoW.Object);

            var result = service.FindRent(2);
            var expected = rentsList.Single(r => r.RentId == 2);

            Assert.AreEqual(expected, result);
        }

        #endregion


        #region AddRent

        [TestMethod]
        public void can_add_new_rent()
        {
            mockRepo.Setup(m => m.Add(It.IsAny<Rent>())).Callback((Rent rent) => rentsList.Add(rent));
            mockUoW.Setup(m => m.RentRepository).Returns(mockRepo.Object);

            RentService service = new RentService(mockUoW.Object);
            Rent newRent = new Rent();
            service.AddRent(newRent);

            Assert.IsTrue(rentsList.Contains(newRent));
            Assert.AreEqual(5, rentsList.Count());
            mockRepo.Verify(m => m.Add(newRent), Times.Once);
        }

        #endregion


        #region DelteRent

        [TestMethod]
        public void can_delete_rent()
        {
            mockRepo.Setup(m => m.Delete(It.IsAny<Rent>())).Callback((Rent rent) =>
                {
                    var rentToDelete = rentsList.Where(r => r.RentId == 1).Single();
                    rentsList.Remove(rentToDelete);
                });
            mockUoW.Setup(m => m.RentRepository).Returns(mockRepo.Object);

            RentService service = new RentService(mockUoW.Object);
            service.DeleteRent(1);

            Assert.AreEqual(3, rentsList.Count());
            Assert.AreEqual(null, rentsList.Find(r => r.RentId == 1));
            mockRepo.Verify(m => m.Delete(It.IsAny<Rent>()), Times.Once);
        }

        #endregion


        #region RentExists

        [TestMethod]
        public void RentExists_return_true_if_rent_exists()
        {
            mockRepo.Setup(m => m.Get()).Returns(rentsList.AsQueryable());
            mockUoW.Setup(m => m.RentRepository).Returns(mockRepo.Object);

            RentService service = new RentService(mockUoW.Object);

            var result = service.RentExists(1, 1);

            Assert.IsTrue(result);
        }

        #endregion
    }
}
