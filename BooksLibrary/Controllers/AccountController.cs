using AutoMapper;
using BooksLibrary.Business.Models;
using BooksLibrary.Business.SecurityServices;
using BooksLibrary.Business.Service;
using BooksLibrary.Models.AccountViewModel;
using PagedList;
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BooksLibrary.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private const string defaultUserRole = "Użytkownik";
        private const string adminEmailAddress = "libraryadmin@wp.pl";

        private readonly IUserService userService;
        private readonly IRentService rentService;
        private IRoleService roleService;
        private IEmailService emailService;
        private IMapper mapper;
        public AccountController(IUserService userService, IRentService rentService, IRoleService roleService, IEmailService emailService, IMapper mapper)
        {
            this.userService = userService;
            this.rentService = rentService;
            this.roleService = roleService;
            this.emailService = emailService;
            this.mapper = mapper;
        }

        public ActionResult Users(int? page, string searchUser)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            int totalCount;

            ViewBag.SearchUser = searchUser;

            IEnumerable<UserModel> query;

            if (!string.IsNullOrEmpty(searchUser))
            {
                query = userService.GetAllUsers((u => u.FirstName.ToUpper().Contains(searchUser.ToUpper()) ||
                    (u.LastName.ToUpper().Contains(searchUser.ToUpper())) ||
                    (u.UserName.ToUpper().Contains(searchUser.ToUpper()))),
                    o => o.OrderBy(u => u.UserId), out totalCount, pageNumber, pageSize);
            }
            else
            {
                query = userService.GetAllUsers(null, o => o.OrderBy(u => u.UserId), out totalCount, pageNumber, pageSize);
            }

            IEnumerable<UsersViewModel> users = mapper.Map<IEnumerable<UsersViewModel>>(query);
            var usersPaged = new StaticPagedList<UsersViewModel>(users, pageNumber, pageSize, totalCount);

            return View(usersPaged);
        }

        public ActionResult UserSettings(string userName)
        {
            UserModel user = userService.GetByUserName(userName);

            if (user == null)
                return HttpNotFound();

            UserAccountSettingsViewModel userSettings = mapper.Map<UserAccountSettingsViewModel>(user);
            userSettings.UserRoles = roleService.GetRolesForUser(userName);

            Address userAddress = userService.GetUserAddressByUserId(user.UserId);
            AddressViewModel addressModel = mapper.Map<AddressViewModel>(userAddress);
            userSettings.UserAddress = addressModel;

            TempData["userName"] = userName;

            return View(userSettings);
        }

        public ActionResult DisableUserAccount(string userName)
        {
            EnableOrDisableAccountViewModel model = new EnableOrDisableAccountViewModel();
            model.UserName = userName;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DisableUserAccount")]
        public ActionResult DisableUserAccountConfirm(EnableOrDisableAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserName == userService.CurrentUserName())
                {
                    ModelState.AddModelError("", "Nie możesz zablokować konta na które jesteś zalogowany!");
                    return View(model);
                }
                else
                {
                    userService.DisableUser(model.UserName, model.LockReason, false);
                    TempData["message"] = "Konto użytkownika zostało zablokowane";
                    return RedirectToAction("UserSettings", new { userName = model.UserName });
                }
            }
            return View(model);

        }

        public ActionResult EnableUserAccount(int userId)
        {
            UserModel user = userService.GetUserById(userId);
            EnableOrDisableAccountViewModel model = new EnableOrDisableAccountViewModel();
            model.UserName = user.UserName;
            model.LockReason = user.LockAccountReason.Reason;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("EnableUserAccount")]
        public ActionResult EnableUserAccountConfirm(EnableOrDisableAccountViewModel model)
        {
            userService.EnableUser(model.UserId);
            TempData["message"] = "Konto użytkownika zostało odblokowane";
            return RedirectToAction("UserSettings", new { userName = model.UserName });
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool userIsEnabled = true; //by default user account is not disabled by admin
                try
                {
                    string confirmationToken = userService.RegisterUser(model.UserName, model.FirstName, model.LastName, model.Email, model.Gender, userIsEnabled, model.Password);

                    roleService.AddRoleToUser(model.UserName, defaultUserRole);

                    dynamic email = new Email("RegisterConfirmationEmail");
                    email.To = model.Email;
                    email.UserName = model.UserName;
                    email.ConfirmationToken = confirmationToken;
                    emailService.Send(email);

                    return RedirectToAction("ConfirmAccountInfo");
                }
                catch (MembershipCreateUserException ex)
                {
                    ModelState.AddModelError("", ErrorCodeToString(ex.StatusCode));
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ConfirmAccountInfo()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfirmAccount(string confirmationToken)
        {
            if (userService.ConfirmAccount(confirmationToken))
            {
                return RedirectToAction("ConfirmationSuccess");
            }
            else
            {
                return RedirectToAction("ConfirmationFailure");
            }
        }

        [AllowAnonymous]
        public ActionResult ConfirmationSuccess()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfirmationFailure()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResendActivationEmail()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResendActivationEmail(ResendActivationEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                string token = userService.GetAccountConfirmationToken(model.Email);

                dynamic email = new Email("ResendEmail");
                email.To = model.Email;
                email.ComfirmationToken = token;
                emailService.Send(email);
                return RedirectToAction("ResendEmailConfirm");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResendEmailConfirm()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult LostPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LostPassword(LostPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string passwordResetToken = userService.GeneratePasswordResetToken(model.Email);

                dynamic email = new Email("ResetPassword");
                email.To = model.Email;
                email.PasswordResetToken = passwordResetToken;
                emailService.Send(email);

                return RedirectToAction("ResetPasswordConfirm");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirm()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string id)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            model.PasswordResetToken = id;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = userService.ResetPassword(model.PasswordResetToken, model.NewPassword);
                if (success)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Błąd. Nie można zapisać nowego hasła");
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LogUserViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (userService.AccountExists(model.UserName))
                {
                    if (!userService.UserIsEnabled(model.UserName))
                    {
                        return RedirectToAction("AccountIsDisabled", new { userName = model.UserName });
                    }

                    if (!userService.AccountIsConfirm(model.UserName))
                    {
                        ModelState.AddModelError("accountIsNotConfirm", "Twoje konto nie zostało jeszcze aktywowane. Zaloguj się na swoją skrzynkę pocztową i kliknij w przesłany link aktywacyjny. " +
                        "Jeśli w Twojej skrzynce nie ma żadnej wiadomości dotyczącej aktywacji konta, możesz wygenerować ją ponownie.");
                        ViewBag.ResendActivationLinkIsVisible = "showResendEmailLink";
                    }

                    else if (userService.LogUser(model.UserName, model.Password, model.RememberMe))
                    {
                        return RedirectToLocal(returnUrl);
                    }

                    else
                    {
                        ModelState.AddModelError("", "Niepoprawne hasło lub nazwa użytkownika");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Niepoprawne hasło lub nazwa użytkownika");
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult AccountIsDisabled(string userName)
        {
            UserModel user = userService.GetByUserName(userName);
            LockAccountReasonViewModel lockReasonModel = new LockAccountReasonViewModel();
            lockReasonModel.LockReason = user.LockAccountReason.Reason;
            lockReasonModel.ReturnBookDateExpired = user.LockAccountReason.ReturnBookDateExpired;
            return View(lockReasonModel);
        }

        [AllowAnonymous]
        public ActionResult SendListWithNotReturnedBooks()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SendListWithNotReturnedBooks(GetEmailAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var rents = rentService.GetAllRents(r => r.ReturnDate < DateTime.Now).ToList();

                dynamic email = new Email("NotReturnedBooks");
                email.To = model.Email;
                email.Subject = "Lista niezwróconych książek";
                email.Message = rents;
                emailService.Send(email);

                return RedirectToAction("EmailWasSent");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult EmailWasSent()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ContactWithAdministrator()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ContactWithAdministrator(ContactWithAdministratorViewModel model)
        {
            if (ModelState.IsValid)
            {
                dynamic email = new Email("EmailToAdmin");
                email.To = adminEmailAddress;
                email.UserName = model.UserName;
                email.From = model.Email;
                email.Subject = model.MessageSubject;
                email.Message = model.Message;
                emailService.Send(email);

                return RedirectToAction("EmailToAdminWasSent");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult EmailToAdminWasSent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            userService.LogOutUser();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminDeleteUserAccount(string userName)
        {
            UserModel user = userService.GetByUserName(userName);
            UserAccountSettingsViewModel userModel = mapper.Map<UserAccountSettingsViewModel>(user);
            return View(userModel);
        }

        [HttpPost]
        [ActionName("AdminDeleteUserAccount")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AdminDeleteUserAccountConfirm(string userName)
        {
            UserModel user = userService.GetByUserName(userName);
            if (userName == userService.CurrentUserName())
            {
                ModelState.AddModelError("", "Próbujesz usunąć konto, na które jesteś aktualnie zalogowany. Jeśli chcesz usunąć swoje konto, kliknij w swoją nazwę użytkownika w prawym górnym rogu i wybierz 'Usuń konto'.");
            }
            else if (rentService.RentExists(user.UserId))
            {
                ModelState.AddModelError("", "Nie można usunąć wybranego użytkownika, ponieważ nie zwrócił wypożyczonych przez niego książek!");
            }
            else
            {
                userService.Delete(user);
                TempData["message"] = "Konto użytkownika zostało usunięte";
                return RedirectToAction("Users");
            }
            UserAccountSettingsViewModel userModel = mapper.Map<UserAccountSettingsViewModel>(user);
            return View(userModel);
        }

        public ActionResult Manage()
        {
            string userName = userService.CurrentUserName();
            UserModel user = userService.GetByUserName(userName);

            if (user == null)
                return HttpNotFound();

            AccountManagementViewModel accountManagement = mapper.Map<AccountManagementViewModel>(user);
            accountManagement.UserRoles = roleService.GetRolesForUser(userName);

            Address userAddress = userService.GetUserAddressByUserId(user.UserId);
            AddressViewModel addressModel = mapper.Map<AddressViewModel>(userAddress);
            accountManagement.UserAddress = addressModel;

            return View(accountManagement);
        }

        public ActionResult DeleteAccount()
        {
            string userName = userService.CurrentUserName();
            UserModel user = userService.GetByUserName(userName);
            UserAccountSettingsViewModel userModel = mapper.Map<UserAccountSettingsViewModel>(user);
            return View(userModel);
        }

        [HttpPost]
        [ActionName("DeleteAccount")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccountConfirm()
        {
            string userName = userService.CurrentUserName();
            UserModel user = userService.GetByUserName(userName);
            if (rentService.RentExists(user.UserId))
            {
                ModelState.AddModelError("", "Nie możesz usunąć konta, ponieważ nie zwróciłeś/zwróciłaś jeszcze wszystkich książek!");
            }
            else
            {
                userService.LogOutUser();
                userService.Delete(user);
                return RedirectToAction("InfoAfterAccountDelete");
            }
            UserAccountSettingsViewModel userModel = mapper.Map<UserAccountSettingsViewModel>(user);
            return View(userModel);
        }

        [AllowAnonymous]
        public ActionResult InfoAfterAccountDelete()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userName = userService.CurrentUserName();
                bool success = userService.ChangePassword(userName, model.ActualPassword, model.NewPassword);
                if (success)
                {
                    TempData["message"] = "Hasło zostało zmienione!";
                    return RedirectToAction("Manage");
                }
                else
                {
                    ModelState.AddModelError("", "Hasła są niepoprawne! Nie można zapisać zmian!");
                }
            }
            return View(model);
        }


        public ActionResult EditPersonalData()
        {
            string userName = userService.CurrentUserName();
            UserModel user = userService.GetByUserName(userName);
            EditPersonalDataViewModel model = mapper.Map<EditPersonalDataViewModel>(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPersonalData(EditPersonalDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel user = mapper.Map<UserModel>(model);
                userService.EditUserData(user);
                TempData["message"] = "Zmiany zostały zapisane!";
                return RedirectToAction("Manage");
            }

            return View(model);
        }

        public ActionResult EditUserAddress()
        {
            int userId = userService.CurrentUserId();
            Address userAddress = userService.GetUserAddressByUserId(userId);
            AddressViewModel addressModel = mapper.Map<AddressViewModel>(userAddress);
            return View(addressModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserAddress(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                Address address = mapper.Map<Address>(model);
                userService.EditAddress(address);

                TempData["message"] = "Zmiany zostały zapisane!";
                return RedirectToAction("Manage");
            }
            return View(model);
        }

        public ActionResult SendEmailToUser(string userEmail)
        {
            SendEmailToUserViewModel sendEmailModel = new SendEmailToUserViewModel();
            sendEmailModel.Email = userEmail;
            return View(sendEmailModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmailToUser(SendEmailToUserViewModel sendEmailModel)
        {
            if (ModelState.IsValid)
            {
                dynamic email = new Email("EmailToUser");
                email.To = sendEmailModel.Email;
                email.From = adminEmailAddress;
                email.Subject = sendEmailModel.MessageSubject;
                email.Message = sendEmailModel.Message;
                emailService.Send(email);

                TempData["message"] = "Wiadomość została wysłana!";
                return RedirectToAction("Users");
            }

            return View(sendEmailModel);
        }

        public ActionResult AddAddress()
        {
            int userId = userService.CurrentUserId();
            Address address = new Address() { UserId = userId };
            AddressViewModel model = mapper.Map<AddressViewModel>(address);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAddress(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                Address address = mapper.Map<Address>(model);
                userService.AddAddress(address);
                TempData["message"] = "Adres został zapisany!";
                return RedirectToAction("Manage");
            }
            return View();
        }


        #region helpers

        public ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Nazwa użytkownika jest już zajęta.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Adres e-mail jest już zajęty.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Hasło jest niepoprawne.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Adres e-mail jest niepoprawny.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Nazwa użytkownika jest niepoprawna.";

                default:
                    return "Błąd! Proszę spróbować ponownie.";
            }
        }

        #endregion


        protected override void Dispose(bool disposing)
        {
            userService.Dispose();
            rentService.Dispose();
            base.Dispose(disposing);
        }
    }
}
