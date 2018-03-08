using BooksLibrary.Business.Models;
using BooksLibrary.Business.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace BooksLibrary.Business.Service
{
    public class RentService : IRentService
    {
        private readonly IUnitOfWork unitOfWork;
        public RentService(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public IEnumerable<Rent> GetAllRents()
        {
            return unitOfWork.RentRepository.Get();
        }

        public IEnumerable<Rent> GetAllRents(System.Linq.Expressions.Expression<Func<Rent, bool>> filter)
        {
            return unitOfWork.RentRepository.Get().Where(filter);
        }

        public IEnumerable<Rent> GetAllRents(System.Linq.Expressions.Expression<Func<Rent, bool>> filter, string[] includedProperties)
        {
            var query = unitOfWork.RentRepository.Get();

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

        public IEnumerable<Rent> GetAllRents(System.Linq.Expressions.Expression<Func<Rent, bool>> filter, string[] includedProperties, Func<IQueryable<Rent>, IOrderedQueryable<Rent>> orderBy, out int totalCount, int? page = null, int? pageSize = null)
        {
            IQueryable<Rent> query = unitOfWork.RentRepository.Get();
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

        public Rent FindRent(int rentId)
        {
            return unitOfWork.RentRepository.GetById(rentId);
        }

        public void AddRent(Rent rent)
        {
            unitOfWork.RentRepository.Add(rent);
            unitOfWork.Commit();
        }

        public void DeleteRent(int rentId)
        {
            Rent rent = unitOfWork.RentRepository.GetById(rentId);
            unitOfWork.RentRepository.Delete(rent);
            unitOfWork.Commit();
        }

        public bool RentExists(int userId, int bookId)
        {
            return unitOfWork.RentRepository.Get().Where(u => u.UserId == userId).Any(b => b.BookId == bookId);
        }

        public bool RentExists(int userId)
        {
            return unitOfWork.RentRepository.Get().Where(u => u.UserId == userId).Any();
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }
    }
}
