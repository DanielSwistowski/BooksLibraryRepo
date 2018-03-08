using BooksLibrary.Business.Models;
using BooksLibrary.Business.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Service
{
    public class ManagementService : IManagementService
    {
        private readonly IUnitOfWork unitOfWork;
        public ManagementService(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public bool ReleaseBook(int reservationId, int rentTime)
        {
            Reservation reservation = unitOfWork.ReservationRepository.GetById(reservationId);

            if (reservation != null)
            {
                Rent rent = new Rent();
                rent.BookId = reservation.BookId;
                rent.RentDate = DateTime.Now;
                rent.UserId = reservation.UserId;
                rent.ReturnDate = DateTime.Now.AddDays(rentTime);

                unitOfWork.ReservationRepository.Delete(reservation);
                unitOfWork.RentRepository.Add(rent);
                unitOfWork.Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ReturnBook(int rentId)
        {
            Rent rent = unitOfWork.RentRepository.GetById(rentId);
            Book book = unitOfWork.BookRepository.GetById(rent.BookId);
            Reminder reminder = unitOfWork.ReminderRepository.GetById(rentId);
            if (reminder != null)
            {
                unitOfWork.ReminderRepository.Delete(reminder);
            }
            book.Quantity = book.Quantity + 1;
            unitOfWork.BookRepository.Update(book);
            unitOfWork.RentRepository.Delete(rent);
            unitOfWork.Commit();
        }

        public bool ShouldEnableUserAccount(int userId) //check if user return all books with expired return date
        {
            UserModel user = unitOfWork.UserRepository.GetById(userId);
            if (!user.UserIsEnabled)
            {
                LockAccountReason lockReason = unitOfWork.LockAccountReasonRepository.Get().Where(u => u.UserId == userId).Single();
                if (lockReason.ReturnBookDateExpired)
                {
                    var rentsWithExpiredReturnDate = unitOfWork.RentRepository.Get().Where(r => r.UserId == userId && r.ReturnDate < DateTime.Now).ToList();

                    if (rentsWithExpiredReturnDate.Count != 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }
    }
}
