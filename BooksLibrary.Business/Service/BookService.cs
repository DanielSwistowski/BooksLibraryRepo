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
    public class BookService : IBookService
    {
        private readonly IUnitOfWork unitOfWork;
        public BookService(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return unitOfWork.BookRepository.Get();
        }

        public IEnumerable<Book> GetAllBooks(Expression<Func<Book, bool>> filter)
        {
            return unitOfWork.BookRepository.Get().Where(filter);
        }

        public IEnumerable<Book> GetAllBooks(Expression<Func<Book, bool>> filter, string[] includedProperties)
        {
            var query = unitOfWork.BookRepository.Get().Where(filter);

            if (includedProperties != null)
            {
                for (int i = 0; i < includedProperties.Count(); i++)
                {
                    query = query.Include(includedProperties[i]);
                }
            }

            return query;
        }

        public IEnumerable<Book> GetAllBooks(Expression<Func<Book, bool>> filter, string[] includedProperties,
            Func<IQueryable<Book>, IOrderedQueryable<Book>> orderBy, out int totalCount, int? page = null, int? pageSize = null)
        {
            IQueryable<Book> query = unitOfWork.BookRepository.Get();

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

        public Book FindBook(int bookId)
        {
            return unitOfWork.BookRepository.GetById(bookId);
        }

        public void AddBook(Book book)
        {
            unitOfWork.BookRepository.Add(book);
            unitOfWork.Commit();
        }

        public void UpdateBook(Book book)
        {
            unitOfWork.BookRepository.Update(book);
            unitOfWork.Commit();
        }

        public void DeleteBook(int bookId)
        {
            Book book = unitOfWork.BookRepository.GetById(bookId);
            unitOfWork.BookRepository.Delete(book);
            unitOfWork.Commit();
        }

        public DateTime GetPropablyBookAvailableDate(int bookId)
        {
            var date = default(DateTime);
            var availableDateFromReservations = unitOfWork.ReservationRepository.Get().Where(b => b.BookId == bookId).OrderBy(o => o.DateOfReceipt).Select(d => d.DateOfReceipt).FirstOrDefault();
            var availableDateFromRents = unitOfWork.RentRepository.Get().Where(b => b.BookId == bookId).OrderBy(o => o.ReturnDate).Select(s => s.ReturnDate).FirstOrDefault();

            if (availableDateFromRents != default(DateTime) && availableDateFromReservations != default(DateTime))
            {
                if (availableDateFromRents <= availableDateFromReservations)
                {
                    date = availableDateFromRents;
                }
                else
                {
                    date = availableDateFromReservations;
                }
            }
            else if (availableDateFromRents != default(DateTime) && availableDateFromReservations == default(DateTime))
            {
                date = availableDateFromRents;
            }
            else
            {
                date = availableDateFromReservations;
            }

            return date;
        }

        public bool BookIsAvailable(int bookId)
        {
            Book book = unitOfWork.BookRepository.GetById(bookId);
            if (book.Quantity > 0)
                return true;
            else
                return false;
        }

        public bool BookExistsInRents(int bookId)
        {
            return unitOfWork.RentRepository.Get().Where(b => b.BookId == bookId).Any();
        }

        public bool BookExistsInReservations(int bookId)
        {
            return unitOfWork.ReservationRepository.Get().Where(b => b.BookId == bookId).Any();
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }
    }
}
