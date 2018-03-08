using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using BooksLibrary.Business.Models;
using WebMatrix.WebData;
using System.Web.Security;
using BooksLibrary.Business.Service;
using BooksLibrary.Business.UnitOfWork;

namespace BooksLibrary.Business.SecurityServices
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        public UserService(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }


        public IEnumerable<UserModel> GetAllUsers(Expression<Func<UserModel, bool>> filter, Func<IQueryable<UserModel>, IOrderedQueryable<UserModel>> orderBy, out int totalCount, int? page = null, int? pageSize = null)
        {
            IQueryable<UserModel> query = unitOfWork.UserRepository.Get();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            totalCount = query.Count();

            if (orderBy != null)
            {
                query = orderBy(query);

                if (page != null)
                {
                    query = query.Skip(((int)page - 1) * (int)pageSize);
                }

                if (pageSize != null)
                {
                    query = query.Take((int)pageSize);
                }
            }

            return query;
        }

        public string RegisterUser(string userName, string firstName, string lastName, string email, Gender gender, bool userIsEnabled, string password)
        {
            if (EmailIsNotAvailable(email))
                throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateEmail);

            string registrationToken = WebSecurity.CreateUserAndAccount(userName, password, new { firstName, lastName, email, gender, userIsEnabled }, true);
            return registrationToken;
        }

        private bool EmailIsNotAvailable(string email)
        {
            return unitOfWork.UserRepository.Get().Where(u => u.Email == email).Any();
        }

        public bool ConfirmAccount(string confirmationToken)
        {
            return WebSecurity.ConfirmAccount(confirmationToken);
        }

        public bool LogUser(string userName, string password, bool persistCookie = false)
        {
            return WebSecurity.Login(userName, password, persistCookie);
        }

        public bool AccountIsConfirm(string userName)
        {
            return WebSecurity.IsConfirmed(userName);
        }

        public string GetAccountConfirmationToken(string userEmail)
        {      
            return unitOfWork.MembershipRepository.GetAccountConfirmationToken(userEmail);
        }

        public string GeneratePasswordResetToken(string userEmail)
        {
            string userName = unitOfWork.UserRepository.Get().Where(u => u.Email == userEmail).Select(u => u.UserName).Single();
            return WebSecurity.GeneratePasswordResetToken(userName);
        }

        public bool ResetPassword(string token, string password)
        {
            return WebSecurity.ResetPassword(token, password);
        }

        public void LogOutUser()
        {
            WebSecurity.Logout();
        }

        public void DisableUser(string userName, string lockReason, bool returnBookDateExpired)
        {
            UserModel user = unitOfWork.UserRepository.Get().Where(u => u.UserName == userName).Single();
            user.UserIsEnabled = false;

            LockAccountReason reason = new LockAccountReason();
            reason.Reason = lockReason;
            reason.UserId = user.UserId;
            reason.ReturnBookDateExpired = returnBookDateExpired;

            unitOfWork.UserRepository.Update(user);
            unitOfWork.LockAccountReasonRepository.Add(reason);
            unitOfWork.Commit();            
        }

        public void EnableUser(int userId)
        {
            UserModel user = unitOfWork.UserRepository.Get().Where(u => u.UserId == userId).Single();
            user.UserIsEnabled = true;

            LockAccountReason reason = unitOfWork.LockAccountReasonRepository.Get().Where(u => u.UserId == userId).Single();

            unitOfWork.UserRepository.Update(user);
            unitOfWork.LockAccountReasonRepository.Delete(reason);
            unitOfWork.Commit();
        }

        public bool UserIsEnabled(string userName)
        {
            UserModel user = unitOfWork.UserRepository.Get().Where(u => u.UserName == userName).Single();
            if (user.UserIsEnabled)
                return true;
            else
                return false;
        }

        public UserModel GetByUserName(string userName)
        {
            return unitOfWork.UserRepository.Get().Where(u => u.UserName == userName).Single();
        }

        public UserModel GetUserById(int userId)
        {
            return unitOfWork.UserRepository.GetById(userId);
        }

        public void Delete(UserModel user)
        {
            //delete user reservations
            var reservations = unitOfWork.ReservationRepository.Get().Where(u => u.UserId == user.UserId).ToList();
            if (reservations.Count != 0)
            {
                foreach (var reservation in reservations)
                {
                    Book book = unitOfWork.BookRepository.GetById(reservation.BookId);
                    book.Quantity = book.Quantity + 1;
                    unitOfWork.BookRepository.Update(book);
                    unitOfWork.ReservationRepository.Delete(reservation);
                }
            }

            //if account is disabled, delete reason why is disabled
            if (!UserIsEnabled(user.UserName))
            {
                LockAccountReason reason = unitOfWork.LockAccountReasonRepository.Get().Where(u => u.UserId == user.UserId).Single();
                unitOfWork.LockAccountReasonRepository.Delete(reason);
            }

            //delete user address
            Address userAddress = unitOfWork.AddressRepository.GetById(user.UserId);
            if (userAddress != null)
            {
                unitOfWork.AddressRepository.Delete(userAddress);
            }

            unitOfWork.Commit();

            //delete user roles
            if (Roles.GetRolesForUser(user.UserName).Count() > 0)
            {
                Roles.RemoveUserFromRoles(user.UserName, Roles.GetRolesForUser(user.UserName));
            }

            //delete user
            ((SimpleMembershipProvider)System.Web.Security.Membership.Provider).DeleteAccount(user.UserName);
            ((SimpleMembershipProvider)System.Web.Security.Membership.Provider).DeleteUser(user.UserName, true);
        }

        public bool AccountExists(string userName)
        {
            return WebSecurity.UserExists(userName);
        }

        public bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            return WebSecurity.ChangePassword(userName, currentPassword, newPassword);
        }

        public void EditUserData(UserModel user)
        {
            UserModel baseUser = unitOfWork.UserRepository.GetById(user.UserId);
            baseUser.FirstName = user.FirstName;
            baseUser.LastName = user.LastName;
            baseUser.Gender = user.Gender;
            unitOfWork.UserRepository.Update(baseUser);
            unitOfWork.Commit();
        }

        public string CurrentUserName()
        {
            return WebSecurity.CurrentUserName;
        }

        public int CurrentUserId()
        {
            return WebSecurity.CurrentUserId;
        }

        public bool UserAddressExists(int userId)
        {
            return unitOfWork.AddressRepository.Get().Where(u => u.UserId == userId).Any();
        }

        public void AddAddress(Address address)
        {
            unitOfWork.AddressRepository.Add(address);
            unitOfWork.Commit();
        }

        public void EditAddress(Address address)
        {
            unitOfWork.AddressRepository.Update(address);
            unitOfWork.Commit();
        }

        public Address GetUserAddressByUserId(int userId)
        {
            return unitOfWork.AddressRepository.GetById(userId);
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }
    }
}