using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BooksLibrary.Business.SecurityServices;
using BooksLibrary.Business.Service;
using Postal;
using AutoMapper;
using BooksLibrary.Models.AccountViewModel;
using System.Collections.Generic;
using BooksLibrary.Business.Models;
using BooksLibrary.Controllers;
using System.Web.Mvc;
using System.Linq;
using PagedList;
using System.Web.Security;
using System.Web;
using System.Web.Routing;

namespace BooksLibrary.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        private Mock<IUserService> mockUserService = null;
        private Mock<IRentService> mockRentService = null;
        private Mock<IRoleService> mockRoleService = null;
        private Mock<IEmailService> mockEmailService = null;
        private Mock<IMapper> mockMapper = null;

        public AccountControllerTest()
        {
            mockUserService = new Mock<IUserService>();
            mockRentService = new Mock<IRentService>();
            mockRoleService = new Mock<IRoleService>();
            mockEmailService = new Mock<IEmailService>();
            mockMapper = new Mock<IMapper>();
        }


        #region Users

        [TestMethod]
        public void Users_return_users_list()
        {
            int totalCount;
            int pageNumber = 1;
            int pageSize = 10;
            mockUserService.Setup(m => m.GetAllUsers(null, It.IsAny<Func<IQueryable<UserModel>, IOrderedQueryable<UserModel>>>(), out totalCount, pageNumber, pageSize));

            List<UsersViewModel> usersList = new List<UsersViewModel>();
            usersList.Add(new UsersViewModel() { UserId = 1, UserName = "DanielSwistowski", UserFullName = "Daniel Świstowski", Email = "danielswistowski@wp.pl", UserIsEnabled = true });
            usersList.Add(new UsersViewModel() { UserId = 1, UserName = "JanKowalski", UserFullName = "Jan Kowalski", Email = "jankowalski@wp.pl", UserIsEnabled = true });
            usersList.Add(new UsersViewModel() { UserId = 1, UserName = "TomaszNowak", UserFullName = "Tomasz Nowak", Email = "tomasznowak@wp.pl", UserIsEnabled = true });

            mockMapper.Setup(m => m.Map<IEnumerable<UsersViewModel>>(It.IsAny<UserModel>())).Returns(usersList);

            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);

            var result = controller.Users(null, string.Empty) as ViewResult;
            IEnumerable<UsersViewModel> users = (IEnumerable<UsersViewModel>)result.Model;

            CollectionAssert.AreEqual(usersList, users.ToList());
        }

        #endregion

        #region UserSettings

        [TestMethod]
        public void UserSettings_return_HttpNotFound_if_user_is_not_found()
        {
            UserModel user = null;
            mockUserService.Setup(m => m.GetByUserName(It.IsAny<string>())).Returns(user);

            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);

            var result = controller.UserSettings(It.IsAny<string>()) as HttpNotFoundResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        #endregion

        #region DisableUserAccountConfirm

        [TestMethod]
        public void DisableUserAccountConfirm_return_model_state_error_if_model_state_is_not_valid()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");

            var result = controller.DisableUserAccountConfirm(It.IsAny<EnableOrDisableAccountViewModel>()) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void DisableUserAccountConfirm_return_model_state_error_if_seleted_user_is_current_logged_ueser()
        {
            mockUserService.Setup(m => m.CurrentUserName()).Returns("Daniel");
            EnableOrDisableAccountViewModel model = new EnableOrDisableAccountViewModel { UserName = "Daniel" };

            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();

            var result = controller.DisableUserAccountConfirm(model) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Multiple error messages");
            Assert.IsTrue(result.ViewData.ModelState[""].Errors.First().ErrorMessage == "Nie możesz zablokować konta na które jesteś zalogowany!");
        }

        [TestMethod]
        public void DisableUserAccountConfirm_can_disable_user_account_and_redirect_to_correct_url()
        {
            mockUserService.Setup(m => m.CurrentUserName()).Returns("Daniel");
            EnableOrDisableAccountViewModel model = new EnableOrDisableAccountViewModel { UserName = "Jan" };
            mockUserService.Setup(m => m.DisableUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);

            var route = controller.DisableUserAccountConfirm(model) as RedirectToRouteResult;

            mockUserService.Verify(m => m.DisableUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            Assert.IsNotNull(route);
            Assert.AreEqual("UserSettings", route.RouteValues["action"], "Invalid action name");
            Assert.AreEqual(model.UserName, route.RouteValues["userName"], "Invalid user name");
        }

        #endregion

        #region EnableUserAccountConfirm

        [TestMethod]
        public void EnableUserAccountConfirm_can_enable_user_account_and_redirect_to_correct_url()
        {
            mockUserService.Setup(m => m.EnableUser(It.IsAny<int>()));
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            EnableOrDisableAccountViewModel model = new EnableOrDisableAccountViewModel { UserName = "Daniel" };

            var route = controller.EnableUserAccountConfirm(model) as RedirectToRouteResult;

            mockUserService.Verify(m => m.EnableUser(It.IsAny<int>()), Times.Once);
            Assert.IsNotNull(route);
            Assert.AreEqual("UserSettings", route.RouteValues["action"], "Invalid action name");
            Assert.AreEqual(model.UserName, route.RouteValues["userName"], "Invalid user name");
        }

        #endregion

        #region Register

        [TestMethod]
        public void Register_return_model_state_error_if_model_state_is_not_valid()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");

            var result = controller.Register(It.IsAny<RegisterUserViewModel>()) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void Register_catch_exception_and_return_model_state_error_if_user_account_exists()
        {
            mockUserService.Setup(m => m.RegisterUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Gender>(), It.IsAny<bool>(), It.IsAny<string>())).Throws(new MembershipCreateUserException(MembershipCreateStatus.DuplicateUserName));
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();
            RegisterUserViewModel model = new RegisterUserViewModel();

            var result = controller.Register(model) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
            Assert.IsTrue(result.ViewData.ModelState[""].Errors.First().ErrorMessage == "Nazwa użytkownika jest już zajęta.");
        }

        #endregion

        #region ConfirmAccount

        [TestMethod]
        public void ConfirmAccount_redirect_to_correct_url_if_activation_is_successful()
        {
            mockUserService.Setup(m => m.ConfirmAccount(It.IsAny<string>())).Returns(true);
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);

            var route = controller.ConfirmAccount(It.IsAny<string>()) as RedirectToRouteResult;

            Assert.IsNotNull(route);
            Assert.AreEqual("ConfirmationSuccess", route.RouteValues["action"], "Invalid action name");
        }

        #endregion

        #region ResendActivationEmail

        [TestMethod]
        public void ResendActivationEmail_return_model_state_error_if_model_state_is_not_valid()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");

            var result = controller.ResendActivationEmail(It.IsAny<ResendActivationEmailViewModel>()) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void ResendActivationEmail_can_send_enail_and_redirect_to_correct_url()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockUserService.Setup(m => m.GetAccountConfirmationToken(It.IsAny<string>()));
            mockEmailService.Setup(m => m.Send(It.IsAny<Email>()));

            ResendActivationEmailViewModel model = new ResendActivationEmailViewModel();
            var route = controller.ResendActivationEmail(model) as RedirectToRouteResult;

            Assert.IsNotNull(route);
            Assert.AreEqual("ResendEmailConfirm", route.RouteValues["action"], "Invalid action name");
            mockEmailService.Verify(m => m.Send(It.IsAny<Email>()), Times.Once);
        }

        #endregion

        #region LostPassword

        [TestMethod]
        public void LostPassword_return_model_state_error_if_model_state_is_not_valid()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");

            var result = controller.LostPassword(It.IsAny<LostPasswordViewModel>()) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void LostPassword_can_send_email_and_redirect_to_correct_url()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockUserService.Setup(m => m.GetAccountConfirmationToken(It.IsAny<string>()));
            mockEmailService.Setup(m => m.Send(It.IsAny<Email>()));

            LostPasswordViewModel model = new LostPasswordViewModel();
            var route = controller.LostPassword(model) as RedirectToRouteResult;

            Assert.IsNotNull(route);
            Assert.AreEqual("ResetPasswordConfirm", route.RouteValues["action"], "Invalid action name");
            mockEmailService.Verify(m => m.Send(It.IsAny<Email>()), Times.Once);
        }

        #endregion

        #region ResetPassword

        [TestMethod]
        public void ResetPassword_return_model_state_error_if_ResetPassword_method_fail()
        {
            mockUserService.Setup(m => m.ResetPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);

            ResetPasswordViewModel model = new ResetPasswordViewModel();
            controller.ModelState.Clear();
            var result = controller.ResetPassword(model) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void ResetPassword_can_reset_password_and_redirect_to_correct_route()
        {
            mockUserService.Setup(m => m.ResetPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);

            ResetPasswordViewModel model = new ResetPasswordViewModel();
            var route = controller.ResetPassword(model) as RedirectToRouteResult;

            Assert.IsNotNull(route);
            Assert.AreEqual("Login", route.RouteValues["action"], "Invalid action name");
            mockUserService.Verify(m => m.ResetPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion


        #region Login

        [TestMethod]
        public void Login_return_model_state_error_if_model_state_is_not_valid()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");
            LogUserViewModel model = new LogUserViewModel();
            var result = controller.Login(model, It.IsAny<string>()) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void Login_return_correct_returnUrl()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            string url = "sample url";
            var result = controller.Login(url) as ViewResult;

            Assert.AreEqual(url, result.ViewBag.ReturnUrl);
        }

        [TestMethod]
        public void Login_return_model_state_error_if_user_account_not_exists()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockUserService.Setup(m => m.AccountExists(It.IsAny<string>())).Returns(false);
            LogUserViewModel model = new LogUserViewModel();
            controller.ModelState.Clear();
            var result = controller.Login(model, It.IsAny<string>()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void Login_redirect_to_AccountIsDisabled_action_if_user_account_is_disabled()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockUserService.Setup(m => m.AccountExists(It.IsAny<string>())).Returns(true);
            mockUserService.Setup(m => m.UserIsEnabled(It.IsAny<string>())).Returns(false);

            LogUserViewModel model = new LogUserViewModel();
            var result = controller.Login(model, It.IsAny<string>()) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("AccountIsDisabled", result.RouteValues["action"], "Invalid action name");
        }

        [TestMethod]
        public void Login_return_model_state_error_if_user_account_is_not_confirm()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockUserService.Setup(m => m.AccountExists(It.IsAny<string>())).Returns(true);
            mockUserService.Setup(m => m.UserIsEnabled(It.IsAny<string>())).Returns(true);
            mockUserService.Setup(m => m.AccountIsConfirm(It.IsAny<string>())).Returns(false);

            LogUserViewModel model = new LogUserViewModel();
            controller.ModelState.Clear();
            var result = controller.Login(model, It.IsAny<string>()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void Login_can_log_user_and_redirect_to_correct_url()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockUserService.Setup(m => m.AccountExists(It.IsAny<string>())).Returns(true);
            mockUserService.Setup(m => m.UserIsEnabled(It.IsAny<string>())).Returns(true);
            mockUserService.Setup(m => m.AccountIsConfirm(It.IsAny<string>())).Returns(true);
            mockUserService.Setup(m => m.LogUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(true);
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockHttpRequest = new Mock<HttpRequestBase>();
            mockHttpRequest.Setup(m => m.Url).Returns(new Uri("http://localhost:123"));
            mockHttpContext.Setup(m => m.Request).Returns(mockHttpRequest.Object);
            var requestContext = new RequestContext(mockHttpContext.Object, new RouteData());
            controller.Url = new UrlHelper(requestContext);

            LogUserViewModel model = new LogUserViewModel();
            string returnUrl = "sample return url";
            var result = controller.Login(model, returnUrl) as RedirectToRouteResult;

            Assert.AreEqual("Home", result.RouteValues["controller"], "Invalid return url");
            Assert.AreEqual("Index", result.RouteValues["action"], "Invalid return url");
        }
        #endregion


        #region AdminDeleteUserAccountConfirm

        [TestMethod]
        public void AdminDeleteUserAccountConfirm_return_model_state_error_if_user_try_to_delete_his_own_account()
        {
            UserModel user = new UserModel() { UserName = "DanielSwistowski" };
            mockUserService.Setup(m => m.GetByUserName(It.IsAny<string>())).Returns(user);
            string currentUserName = "DanielSwistowski";
            mockUserService.Setup(m => m.CurrentUserName()).Returns(currentUserName);

            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();

            var result = controller.AdminDeleteUserAccountConfirm("DanielSwistowski") as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void AdminDeleteUserAccountConfirm_return_model_state_error_if_selected_user_account_contains_not_returned_books()
        {
            UserModel user = new UserModel();
            mockUserService.Setup(m => m.GetByUserName(It.IsAny<string>())).Returns(user);
            string currentUserName = "DanielSwistowski";
            mockUserService.Setup(m => m.CurrentUserName()).Returns(currentUserName);
            mockRentService.Setup(m => m.RentExists(It.IsAny<int>())).Returns(true);

            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();

            var result = controller.AdminDeleteUserAccountConfirm(It.IsAny<string>()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void AdminDeleteUserAccountConfirm_can_detete_user_account_and_redirect_to_correct_url()
        {
            UserModel user = new UserModel();
            mockUserService.Setup(m => m.GetByUserName(It.IsAny<string>())).Returns(user);
            string currentUserName = "DanielSwistowski";
            mockUserService.Setup(m => m.CurrentUserName()).Returns(currentUserName);
            mockRentService.Setup(m => m.RentExists(It.IsAny<int>())).Returns(false);
            mockUserService.Setup(m => m.Delete(It.IsAny<UserModel>()));

            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);

            var result = controller.AdminDeleteUserAccountConfirm(It.IsAny<string>()) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Users", result.RouteValues["action"], "Invalid action name");
            mockUserService.Verify(m => m.Delete(It.IsAny<UserModel>()), Times.Once);
        }

        #endregion

        #region Manage

        [TestMethod]
        public void Manage_return_HttpNotFound_if_user_is_not_found()
        {
            UserModel user = null;
            mockUserService.Setup(m => m.GetByUserName(It.IsAny<string>())).Returns(user);
            string currentUserName = "DanielSwistowski";
            mockUserService.Setup(m => m.CurrentUserName()).Returns(currentUserName);
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);

            var result = controller.Manage() as HttpNotFoundResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        #endregion


        #region DeleteAccountConfirm

        [TestMethod]
        public void DeleteAccountConfirm_return_model_state_error_if_user_account_contains_not_returned_books()
        {
            UserModel user = new UserModel();
            mockUserService.Setup(m => m.GetByUserName(It.IsAny<string>())).Returns(user);
            mockUserService.Setup(m => m.CurrentUserName());
            mockRentService.Setup(m => m.RentExists(It.IsAny<int>())).Returns(true);
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);

            controller.ModelState.Clear();
            var result = controller.DeleteAccountConfirm() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void DeleteAccountConfirm_can_delete_user_account_and_redirect_to_correct_url()
        {
            UserModel user = new UserModel();
            mockUserService.Setup(m => m.GetByUserName(It.IsAny<string>())).Returns(user);
            mockUserService.Setup(m => m.CurrentUserName());
            mockRentService.Setup(m => m.RentExists(It.IsAny<int>())).Returns(false);
            mockUserService.Setup(m => m.LogOutUser());
            mockUserService.Setup(m => m.Delete(It.IsAny<UserModel>()));
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);

            var result = controller.DeleteAccountConfirm() as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("InfoAfterAccountDelete", result.RouteValues["action"], "Invalid action name");
            mockUserService.Verify(m => m.LogOutUser(), Times.Once);
            mockUserService.Verify(m => m.Delete(It.IsAny<UserModel>()), Times.Once);
        }

        #endregion


        #region ChangePassword

        [TestMethod]
        public void ChangePassword_return_model_state_error_if_model_state_is_not_valid()
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);

            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");
            var result = controller.ChangePassword(model) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void ChangePassword_return_model_state_error_if_ChangePassword_method_fail()
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockUserService.Setup(m => m.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            controller.ModelState.Clear();
            var result = controller.ChangePassword(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void ChangePassword_can_change_password_and_redirect_to_correct_url()
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockUserService.Setup(m => m.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            controller.ModelState.Clear();
            var result = controller.ChangePassword(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Manage", result.RouteValues["action"], "Invalid action name");
            mockUserService.Verify(m => m.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion


        #region EditPersonalData

        [TestMethod]
        public void EditPersonalData_return_model_state_error_if_model_state_is_not_valid()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");

            var result = controller.EditPersonalData(It.IsAny<EditPersonalDataViewModel>()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void EditPersonalData_can_edit_user_data_and_redirect_to_correct_url()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockUserService.Setup(m => m.EditUserData(It.IsAny<UserModel>()));

            var result = controller.EditPersonalData(It.IsAny<EditPersonalDataViewModel>()) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Manage", result.RouteValues["action"], "Invalid action name");
            mockUserService.Verify(m => m.EditUserData(It.IsAny<UserModel>()), Times.Once);
        }

        #endregion


        #region EditUserAddress

        [TestMethod]
        public void EditUserAddress_return_model_state_error_if_model_state_is_not_valid()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");

            var result = controller.EditUserAddress(It.IsAny<AddressViewModel>()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void EditUserAddress_can_edit_user_address_and_redirect_to_correct_url()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockUserService.Setup(m => m.EditAddress(It.IsAny<Address>()));

            var result = controller.EditUserAddress(It.IsAny<AddressViewModel>()) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Manage", result.RouteValues["action"], "Invalid action name");
            mockUserService.Verify(m => m.EditAddress(It.IsAny<Address>()), Times.Once);
        }

        #endregion


        #region SendEmailToUser

        [TestMethod]
        public void SendEmailToUser_return_model_state_error_if_model_state_is_not_valid()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");

            var result = controller.SendEmailToUser(It.IsAny<SendEmailToUserViewModel>()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void SendEmailToUser_can_send_email_and_redirect_to_correct_url()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockEmailService.Setup(m => m.Send(It.IsAny<Email>()));

            SendEmailToUserViewModel model = new SendEmailToUserViewModel();
            var result = controller.SendEmailToUser(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Users", result.RouteValues["action"], "Invalid action name");
            mockEmailService.Verify(m => m.Send(It.IsAny<Email>()), Times.Once);
        }
        
        #endregion


        #region AddAddress

        [TestMethod]
        public void AddAddress_return_model_state_error_if_model_state_is_not_valid()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");

            var result = controller.AddAddress(It.IsAny<AddressViewModel>()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void AddAddress_can_add_user_address_and_redirect_to_correct_url()
        {
            AccountController controller = new AccountController(mockUserService.Object, mockRentService.Object, mockRoleService.Object, mockEmailService.Object, mockMapper.Object);
            mockUserService.Setup(m => m.CurrentUserId()).Returns(It.IsAny<int>());
            mockUserService.Setup(m => m.AddAddress(It.IsAny<Address>()));

            AddressViewModel model = new AddressViewModel();
            var result = controller.AddAddress(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Manage", result.RouteValues["action"], "Invalid action name");
            mockUserService.Verify(m => m.AddAddress(It.IsAny<Address>()), Times.Once);
        }

        #endregion
    }
}