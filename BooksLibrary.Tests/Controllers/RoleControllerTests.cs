using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksLibrary.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BooksLibrary.Business.SecurityServices;
using System.Web.Mvc;
using BooksLibrary.Models.RoleViewModels;
namespace BooksLibrary.Controllers.Tests
{
    [TestClass]
    public class RoleControllerTests
    {
        private Mock<IRoleService> mockRoleService = null;
        string[] roles = { "Admin", "Użytkownik" };
        RoleController controller;

        public RoleControllerTests()
        {
            mockRoleService = new Mock<IRoleService>();
            controller = new RoleController(mockRoleService.Object);
        }

        [TestMethod]
        public void Roles_return_all_roles()
        {
            mockRoleService.Setup(m => m.GetAllRoles()).Returns(roles);

            var result = controller.Roles() as ViewResult;

            Assert.AreEqual(roles, result.Model);
        }

        #region AddRoleToUser

        [TestMethod]
        public void AddRoleToUser_return_HttpNotFound_if_user_name_is_empty_value()
        {
            string userName = "";
            var result = controller.AddRoleToUser(userName) as HttpNotFoundResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public void AddRoleToUser_return_model_state_error_if_model_state_is_not_valid()
        {
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");

            var result = controller.AddRoleToUser(It.IsAny<AddOrDeleteUserRoleViewModel>()) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void AddRoleToUser_return_model_state_error_if_model_state_is_user_account_contain_selected_role()
        {
            mockRoleService.Setup(m => m.UserAccountContainSelectedRole(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            controller.ModelState.Clear();
            AddOrDeleteUserRoleViewModel model = new AddOrDeleteUserRoleViewModel();

            var result = controller.AddRoleToUser(model) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void AddRoleToUser_can_add_new_role_to_user_and_redirect_to_correct_url()
        {
            mockRoleService.Setup(m => m.UserAccountContainSelectedRole(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockRoleService.Setup(m => m.AddRoleToUser(It.IsAny<string>(), It.IsAny<string>()));
            AddOrDeleteUserRoleViewModel model = new AddOrDeleteUserRoleViewModel() { UserName = "Daniel" };

            var result = controller.AddRoleToUser(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Account", result.RouteValues["controller"], "Invalid controller name");
            Assert.AreEqual("UserSettings", result.RouteValues["action"], "Invalid action name");
            Assert.AreEqual(model.UserName, result.RouteValues["userName"], "Invalid user name");
            mockRoleService.Verify(m => m.AddRoleToUser(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        [TestMethod]
        public void RemoveUserRole_return_HttpNotFound_if_user_name_or_role_name_are_empty_value()
        {
            string userName = "";
            string roleName = "Admin";
            var result = controller.RemoveUserRole(userName, roleName) as HttpNotFoundResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public void RemoveUserRole_can_remove_user_role_and_redirect_to_correct_url()
        {
            mockRoleService.Setup(m => m.RemoveRoleFromUser(It.IsAny<string>(), It.IsAny<string>()));
            AddOrDeleteUserRoleViewModel model = new AddOrDeleteUserRoleViewModel() { UserName = "Daniel" };

            var result = controller.RemoveUserRole(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Account", result.RouteValues["controller"], "Invalid controller name");
            Assert.AreEqual("UserSettings", result.RouteValues["action"], "Invalid action name");
            Assert.AreEqual(model.UserName, result.RouteValues["userName"], "Invalid user name");
            mockRoleService.Verify(m => m.RemoveRoleFromUser(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void CreateRole_return_model_state_error_if_model_state_is_not_valid()
        {
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("", "Error");

            var result = controller.CreateRole(It.IsAny<CreateRoleViewModel>()) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void CreateRole_can_create_new_role_and_redirect_to_correct_url()
        {
            mockRoleService.Setup(m => m.CreateRole(It.IsAny<string>()));
            CreateRoleViewModel model = new CreateRoleViewModel();

            var result = controller.CreateRole(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Roles", result.RouteValues["action"], "Invalid action name");
            mockRoleService.Verify(m => m.CreateRole(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void DeleteRole_return_model_state_error_if_InvalidOperationException_occurs()
        {
            mockRoleService.Setup(m => m.DeleteRole(It.IsAny<string>())).Throws(new InvalidOperationException());
            controller.ModelState.Clear();

            var result = controller.DeleteRoleConfirm(It.IsAny<string>()) as ViewResult;

            Assert.IsTrue(result.ViewData.ModelState.Count == 1, "Can't return model state error");
        }

        [TestMethod]
        public void DeleteRoleConfirm_can_delete_role_and_redirect_to_correct_url()
        {
            mockRoleService.Setup(m => m.DeleteRole(It.IsAny<string>()));

            var result = controller.DeleteRoleConfirm(It.IsAny<string>()) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Roles", result.RouteValues["action"], "Invalid action name");
            mockRoleService.Verify(m => m.DeleteRole(It.IsAny<string>()), Times.Once);
        }
    }
}
