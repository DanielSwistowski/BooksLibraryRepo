using BooksLibrary.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Service
{
    public interface IReservationService : IDisposable
    {
        IEnumerable<Reservation> GetAllReservations();

        IEnumerable<Reservation> GetAllReservations(Expression<Func<Reservation, bool>> filter);

        IEnumerable<Reservation> GetAllReservations(Expression<Func<Reservation, bool>> filter, string[] includedProperties);

        IEnumerable<Reservation> GetAllReservations(Expression<Func<Reservation, bool>> filter, string[] includedProperties,
            Func<IQueryable<Reservation>, IOrderedQueryable<Reservation>> orderBy, out int totalCount, int? page = null, int? pageSize = null);

        Reservation FindReservation(int reservationId);

        void AddReservation(Reservation reservation);

        //void UpdateReservation(Reservation reservation);

        void DeleteReservation(int reservationId);

        bool ReservationExists(int userId, int bookId);
    }
}
