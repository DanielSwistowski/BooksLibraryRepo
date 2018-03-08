using BooksLibrary.Business.SecurityServices;
using BooksLibrary.Models.RoleViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BooksLibrary.Controllers
{
    public class RoleController : Controller
    {
        private IRoleService roleService;
        public RoleController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        public ActionResult Roles()
        {
            return View(roleService.GetAllRoles());
        }

        public ActionResult AddRoleToUser(string userName = "")
        {
            if (userName == "")
                return HttpNotFound();

            AddOrDeleteUserRoleViewModel addOrDeleteUserRoleViewModel = new AddOrDeleteUserRoleViewModel();
            addOrDeleteUserRoleViewModel.UserName = userName;
            PopulateRolesDropDownList();
            return View(addOrDeleteUserRoleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRoleToUser(AddOrDeleteUserRoleViewModel addOrDeleteUserRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                if(!roleService.UserAccountContainSelectedRole(addOrDeleteUserRoleViewModel.RoleName,addOrDeleteUserRoleViewModel.UserName))                
                {
                    roleService.AddRoleToUser(addOrDeleteUserRoleViewModel.UserName, addOrDeleteUserRoleViewModel.RoleName);
                    return RedirectToAction("UserSettings", "Account", new { userName = addOrDeleteUserRoleViewModel.UserName });
                }
                else
                {
                    ModelState.AddModelError("", "Użytkownik już posiada wskazane uprawnienia");
                }
            }
            PopulateRolesDropDownList();
            return View(addOrDeleteUserRoleViewModel);
        }

        private void PopulateRolesDropDownList()
        {
            int key = 0;
            var rolesDictionary = new Dictionary<int, string>();
            var roles = roleService.GetAllRoles();
            foreach (var role in roles)
            {
                rolesDictionary.Add(key, role);
                key++;
            }

            ViewBag.Roles = new SelectList(rolesDictionary, "Value", "Value");
        }

        public ActionResult RemoveUserRole(string userName = "", string roleName = "")
        {
            if (userName == "" || roleName == "")
                return HttpNotFound();

            AddOrDeleteUserRoleViewModel addOrDeleteUserRoleViewModel = new AddOrDeleteUserRoleViewModel();
            addOrDeleteUserRoleViewModel.UserName = userName;
            addOrDeleteUserRoleViewModel.RoleName = roleName;

            return View(addOrDeleteUserRoleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveUserRole(AddOrDeleteUserRoleViewModel addOrDeleteUserRoleViewModel)
        {
            roleService.RemoveRoleFromUser(addOrDeleteUserRoleViewModel.UserName, addOrDeleteUserRoleViewModel.RoleName);
            return RedirectToAction("UserSettings", "Account", new { userName = addOrDeleteUserRoleViewModel.UserName });
        }

        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRole(CreateRoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                roleService.CreateRole(viewModel.RoleName);
                TempData["message"] = "Utworzono nowe uprawnienie: " + viewModel.RoleName;
                return RedirectToAction("Roles");
            }
            return View(viewModel);
        }

        public ActionResult DeleteRole(string roleName)
        {
            string roleToDelete = roleName;
            return View((object)roleToDelete);
        }

        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleConfirm(string roleName)
        {
            try
            {
                roleService.DeleteRole(roleName);
                TempData["message"] = "Uprawnienie " + roleName + " zostało usunięte";
                return RedirectToAction("Roles");
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError("", "Nie można usunąć wybranego uprawnienia, ponieważ jest ono przypisane przynajmniej do jednego użytkownika.");
            }
            string roleToDelete = roleName;
            return View((object)roleToDelete);
        }
    }
}