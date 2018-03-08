using BooksLibrary.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Service
{
    public interface IRentService : IDisposable
    {
        IEnumerable<Rent> GetAllRents();

        IEnumerable<Rent> GetAllRents(Expression<Func<Rent, bool>> filter);

        IEnumerable<Rent> GetAllRents(Expression<Func<Rent, bool>> filter, string[] includedProperties);

        IEnumerable<Rent> GetAllRents(Expression<Func<Rent, bool>> filter, string[] includedProperties,
            Func<IQueryable<Rent>, IOrderedQueryable<Rent>> orderBy, out int totalCount, int? page = null, int? pageSize = null);

        Rent FindRent(int rentId);

        void AddRent(Rent rent);

        //void UpdateRent(Rent rent);

        void DeleteRent(int rentId);

        bool RentExists(int userId, int bookId);

        bool RentExists(int userId);
    }
}
