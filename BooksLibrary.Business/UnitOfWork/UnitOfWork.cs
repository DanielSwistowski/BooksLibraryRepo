using BooksLibrary.Business.Models;
using BooksLibrary.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private BookLibraryDataContext context;
        private Repository<Book> bookRepository;
        private Repository<Reservation> reservationRepository;
        private Repository<Rent> rentRepository;
        private Repository<UserModel> userRepository;
        private MembershipRepository membershipRepository;
        private Repository<LockAccountReason> lockAccountReasonRepository;
        private Repository<Reminder> reminderRepository;
        private Repository<Address> addressRepository;
        private Repository<Category> categoryRepository;

        public UnitOfWork()
        {
            context = new BookLibraryDataContext();
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public IRepository<Book> BookRepository
        {
            get 
            {
                if (bookRepository == null)
                    bookRepository = new Repository<Book>(context);
                return bookRepository;
            }
        }

        public IRepository<Reservation> ReservationRepository
        {
            get
            {
                if (reservationRepository == null)
                    reservationRepository = new Repository<Reservation>(context);
                return reservationRepository;
            }
        }

        public IRepository<Rent> RentRepository
        {
            get
            {
                if (rentRepository == null)
                    rentRepository = new Repository<Rent>(context);
                return rentRepository;
            }
        }

        public IRepository<UserModel> UserRepository
        {
            get
            {
                if (userRepository == null)
                    userRepository = new Repository<UserModel>(context);
                return userRepository;
            }
        }

        public IMembershipRepository MembershipRepository
        {
            get
            {
                if(membershipRepository == null)
                    membershipRepository = new MembershipRepository(context);
                return membershipRepository;
            }
        }

        public IRepository<LockAccountReason> LockAccountReasonRepository
        {
            get
            {
                if (lockAccountReasonRepository == null)
                    lockAccountReasonRepository = new Repository<LockAccountReason>(context);
                return lockAccountReasonRepository;
            }
        }

        public IRepository<Reminder> ReminderRepository
        {
            get
            {
                if (reminderRepository == null)
                    reminderRepository = new Repository<Reminder>(context);
                return reminderRepository;
            }
        }

        public IRepository<Address> AddressRepository
        {
            get
            {
                if (addressRepository == null)
                    addressRepository = new Repository<Address>(context);
                return addressRepository;
            }
        }

        public IRepository<Category> CategoryRepository
        {
            get 
            {
                if (categoryRepository == null)
                    categoryRepository = new Repository<Category>(context);
                return categoryRepository;
            }
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
