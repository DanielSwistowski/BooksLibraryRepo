using BooksLibrary.Business.Models;
using BooksLibrary.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        IRepository<Book> BookRepository { get; }

        IRepository<Reservation> ReservationRepository { get; }

        IRepository<Rent> RentRepository { get; }

        IRepository<UserModel> UserRepository { get; }

        IMembershipRepository MembershipRepository { get; }

        IRepository<LockAccountReason> LockAccountReasonRepository { get; }

        IRepository<Reminder> ReminderRepository { get; }

        IRepository<Address> AddressRepository { get; }

        IRepository<Category> CategoryRepository { get; }
    }
}
