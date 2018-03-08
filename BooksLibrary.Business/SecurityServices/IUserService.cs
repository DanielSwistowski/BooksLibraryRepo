using BooksLibrary.Business.Models;
using BooksLibrary.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.SecurityServices
{
    public interface IUserService : IDisposable
    {
        IEnumerable<UserModel> GetAllUsers(Expression<Func<UserModel, bool>> filter, Func<IQueryable<UserModel>, IOrderedQueryable<UserModel>> orderBy, out int totalCount, int? page = null, int? pageSize = null);

        string RegisterUser(string userName, string firstName, string lastName, string email, Gender gender, bool userIsEnabled, string password);

        bool ConfirmAccount(string confirmationToken);

        string GetAccountConfirmationToken(string userEmail);

        bool AccountIsConfirm(string userName);

        bool LogUser(string userName, string password, bool persistCookie = false);

        void LogOutUser();

        void DisableUser(string userName, string lockReason, bool returnBookDateExpired);

        void EnableUser(int userId);

        bool UserIsEnabled(string userName);

        UserModel GetByUserName(string userName);

        UserModel GetUserById(int userId);

        string GeneratePasswordResetToken(string userEmail);

        bool ResetPassword(string token, string password);

        bool AccountExists(string userName);

        void Delete(UserModel user);

        bool ChangePassword(string userName, string currentPassword, string newPassword);

        void EditUserData(UserModel user);

        string CurrentUserName();

        int CurrentUserId();

        bool UserAddressExists(int userId);

        void AddAddress(Address address);

        void EditAddress(Address address);

        Address GetUserAddressByUserId(int userId);
    }
}
