using BooksLibrary.Business.Models;
using BooksLibrary.Business.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace BooksLibrary.Business.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork unitOfWork;
        public ReservationService(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public IEnumerable<Reservation> GetAllReservations()
        {
            return unitOfWork.ReservationRepository.Get();
        }

        public IEnumerable<Reservation> GetAllReservations(Expression<Func<Reservation, bool>> filter)
        {
            return unitOfWork.ReservationRepository.Get().Where(filter);
        }

        public IEnumerable<Reservation> GetAllReservations(Expression<Func<Reservation, bool>> filter, string[] includedProperties)
        {
            var query = unitOfWork.ReservationRepository.Get();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includedProperties != null)
            {
                for (int i = 0; i < includedProperties.Count(); i++)
                {
                    query = query.Include(includedProperties[i]);
                }
            }

            return query;
        }

        public IEnumerable<Reservation> GetAllReservations(Expression<Func<Reservation, bool>> filter, string[] includedProperties, Func<IQueryable<Reservation>, IOrderedQueryable<Reservation>> orderBy, out int totalCount, int? page = null, int? pageSize = null)
        {
            IQueryable<Reservation> query = unitOfWork.ReservationRepository.Get();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includedProperties != null)
            {
                for (int i = 0; i < includedProperties.Count(); i++)
                {
                    query = query.Include(includedProperties[i]);
                }
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

        public Reservation FindReservation(int reservationId)
        {
            return unitOfWork.ReservationRepository.GetById(reservationId);
        }

        public void AddReservation(Reservation reservation)
        {
            Book book = unitOfWork.BookRepository.GetById(reservation.BookId);
            book.Quantity = book.Quantity - 1;
            unitOfWork.BookRepository.Update(book);

            unitOfWork.ReservationRepository.Add(reservation);
            unitOfWork.Commit();
        }

        public bool ReservationExists(int userId, int bookId)
        {
            return unitOfWork.ReservationRepository.Get().Where(u => u.UserId == userId).Any(b => b.BookId == bookId);
        }

        public void DeleteReservation(int reservationId)
        {
            Reservation reservation = unitOfWork.ReservationRepository.GetById(reservationId);
            if (reservation != null)
            {
                Book book = unitOfWork.BookRepository.GetById(reservation.BookId);
                book.Quantity = book.Quantity + 1;
                unitOfWork.BookRepository.Update(book);
                unitOfWork.ReservationRepository.Delete(reservation);
                unitOfWork.Commit();
            }
        }


        public void Dispose()
        {
            unitOfWork.Dispose();
        }
    }
}
