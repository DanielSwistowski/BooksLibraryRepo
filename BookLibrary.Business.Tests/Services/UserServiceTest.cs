using BooksLibrary.Business.Models;
using BooksLibrary.Business.Repository;
using BooksLibrary.Business.SecurityServices;
using BooksLibrary.Business.UnitOfWork;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace BookLibrary.Business.Tests.Services
{
    [TestClass]
    public class UserServiceTest
    {
        private Mock<IUnitOfWork> mockUoW = null;
        private Mock<IRepository<UserModel>> mockUserRepo = null;
        private List<UserModel> usersList = null;
        private Mock<IRepository<LockAccountReason>> mockLockReasonRepo = null;

        public UserServiceTest()
        {
            mockUoW = new Mock<IUnitOfWork>();
            mockUserRepo = new Mock<IRepository<UserModel>>();
            usersList = new List<UserModel>()
            {
                new UserModel{ UserId=1, UserName="DanielSwistowski", FirstName="Daniel", LastName="Świstowski", Email="danielswistowski@wp.pl", Gender = Gender.Male, UserIsEnabled = true},
                new UserModel{ UserId=2, UserName="JanKowalski", FirstName="Jan", LastName="Kowalski", Email="jankowalski@wp.pl", Gender = Gender.Male, UserIsEnabled = true},
                new UserModel{ UserId=3, UserName="AdamFreeman", FirstName="Adam", LastName="Freeman", Email="adamfreeman@wp.pl", Gender = Gender.Male, UserIsEnabled = true},
                new UserModel{ UserId=4, UserName="TomaszLato", FirstName="Tomasz", LastName="Lato", Email="tomaszlato@wp.pl", Gender = Gender.Male, UserIsEnabled = false},
                new UserModel{ UserId=5, UserName="TomaszAdamek", FirstName="Tomasz", LastName="Adamek", Email="tomaszadamek@wp.pl", Gender = Gender.Male, UserIsEnabled = true}
            };
            mockLockReasonRepo = new Mock<IRepository<LockAccountReason>>();
        }


        #region GetAllUsers

        [TestMethod]
        public void GetAllUsers_can_get_all_users_with_filtering_and_paging()
        {
            mockUserRepo.Setup(m => m.Get()).Returns(usersList.AsQueryable());
            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);

            UserService service = new UserService(mockUoW.Object);

            int totalCount;
            int pageSize = 1;
            int pageNumber = 2;

            var result = service.GetAllUsers(u => u.FirstName == "Tomasz", o => o.OrderBy(u => u.UserId), out totalCount, pageNumber, pageSize).ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Adamek", result[0].LastName);
            Assert.AreEqual(2, totalCount);
        }

        #endregion


        #region RegisterUser

        [TestMethod]
        [ExpectedException(typeof(MembershipCreateUserException))]
        public void RegisterUser_throw_MembershipCreateUserException_if_email_address_is_not_available()
        {
            mockUserRepo.Setup(m => m.Get()).Returns(usersList.AsQueryable());
            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);

            UserService service = new UserService(mockUoW.Object);

            service.RegisterUser("", "", "", "danielswistowski@wp.pl", Gender.Male, false, "");
        }

        #endregion


        #region DisableUser

        [TestMethod]
        public void DisableUser_can_disable_user()
        {
            mockUserRepo.Setup(m => m.Get()).Returns(usersList.AsQueryable());
            List<LockAccountReason> lockReasonFakeRepo = new List<LockAccountReason>();
            LockAccountReason lockReason = new LockAccountReason(){ Reason="Test", ReturnBookDateExpired=false, UserId =1 };
            mockLockReasonRepo.Setup(m => m.Add(It.IsAny<LockAccountReason>())).Callback((LockAccountReason reason) => lockReasonFakeRepo.Add(lockReason));
            mockUserRepo.Setup(m => m.Update(It.IsAny<UserModel>())).Callback((UserModel userModel) =>
                {
                    var user = usersList.Where(u => u.UserName == "DanielSwistowski").Single();
                    usersList.Remove(user);
                    usersList.Add(userModel);
                });
            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);
            mockUoW.Setup(m => m.LockAccountReasonRepository).Returns(mockLockReasonRepo.Object);

            UserService service = new UserService(mockUoW.Object);
            service.DisableUser("DanielSwistowski", "Test", false);

            Assert.AreEqual(5, usersList.Count);
            Assert.IsTrue(usersList.Where(u => u.UserName == "DanielSwistowski").Single().UserIsEnabled == false);
            Assert.IsTrue(lockReasonFakeRepo.Contains(lockReason));
        }

        #endregion


        #region EnableUser

        [TestMethod]
        public void EnableUser_can_enable_user()
        {
            mockUserRepo.Setup(m => m.Get()).Returns(usersList.AsQueryable());
            mockUserRepo.Setup(m => m.Update(It.IsAny<UserModel>())).Callback((UserModel userModel) =>
            {
                var user = usersList.Where(u => u.UserId == 4).Single();
                usersList.Remove(user);
                usersList.Add(userModel);
            });

            List<LockAccountReason> lockReasonFakeRepo = new List<LockAccountReason>();
            LockAccountReason lockReason = new LockAccountReason() { Reason = "Test", ReturnBookDateExpired = false, UserId = 4 };
            lockReasonFakeRepo.Add(lockReason);
            mockLockReasonRepo.Setup(m => m.Get()).Returns(lockReasonFakeRepo.AsQueryable());
            mockLockReasonRepo.Setup(m => m.Delete(It.IsAny<LockAccountReason>())).Callback((LockAccountReason reason) =>
                {
                    var reasonToDelete = lockReasonFakeRepo.Where(u => u.UserId == 4).Single();
                    lockReasonFakeRepo.Remove(reasonToDelete);
                });

            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);
            mockUoW.Setup(m => m.LockAccountReasonRepository).Returns(mockLockReasonRepo.Object);

            UserService service = new UserService(mockUoW.Object);
            service.EnableUser(4);

            Assert.AreEqual(5, usersList.Count);
            Assert.IsTrue(usersList.Where(u => u.UserId == 4).Single().UserIsEnabled == true);
            Assert.IsFalse(lockReasonFakeRepo.Contains(lockReason));
        }

        #endregion


        #region UserIsEnabled

        [TestMethod]
        public void UserIsEnabled_returns_true_if_user_is_enabled()
        {
            mockUserRepo.Setup(m => m.Get()).Returns(usersList.AsQueryable());
            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);
            
            UserService service = new UserService(mockUoW.Object);
            var result = service.UserIsEnabled("DanielSwistowski");

            Assert.IsTrue(result);
        }

        #endregion


        #region GetByUserName

        [TestMethod]
        public void GetByUserName_can_get_user_by_user_name()
        {
            mockUserRepo.Setup(m => m.Get()).Returns(usersList.AsQueryable());
            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);

            UserService service = new UserService(mockUoW.Object);
            var result = service.GetByUserName("DanielSwistowski");

            var expected = usersList.Where(u => u.UserName == "DanielSwistowski").Single();

            Assert.AreEqual(expected, result);
        }

        #endregion


        #region GetUserById

        [TestMethod]
        public void GetUserById_can_find_user_by_id()
        {
            mockUserRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns((int i) => usersList.Single(u => u.UserId == i));
            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);

            UserService service = new UserService(mockUoW.Object);
            var result = service.GetUserById(1);
            var expected = usersList.Single(u => u.UserId == 1);

            Assert.AreEqual(expected, result);
        }

        #endregion


        #region EditUserData

        [TestMethod]
        public void EditUserData_can_edit_user_data()
        {
            mockUserRepo.Setup(m => m.GetById(It.IsAny<int>())).Returns((int i) => usersList.Single(u => u.UserId == i));
            mockUserRepo.Setup(m => m.Update(It.IsAny<UserModel>())).Callback((UserModel userModel) =>
            {
                var user = usersList.Where(u => u.UserId == 1).Single();
                usersList.Remove(user);
                usersList.Add(userModel);
            });
            mockUoW.Setup(m => m.UserRepository).Returns(mockUserRepo.Object);

            UserModel updatedUser = new UserModel() { UserId = 1, UserName = "DanielSwistowski", FirstName = "Jan", LastName = "Kowalski", Email = "danielswistowski@wp.pl", Gender = Gender.Male, UserIsEnabled = true };
            UserService service = new UserService(mockUoW.Object);
            service.EditUserData(updatedUser);

            Assert.AreEqual(5, usersList.Count);
            Assert.AreEqual("DanielSwistowski", usersList.Single(u => u.UserId == 1).UserName);
            Assert.AreEqual("Jan", usersList.Single(u => u.UserId == 1).FirstName);
            Assert.AreEqual("Kowalski", usersList.Single(u => u.UserId == 1).LastName);
        }

        #endregion
    }
}
