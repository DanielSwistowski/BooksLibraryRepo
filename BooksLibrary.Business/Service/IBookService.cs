using BooksLibrary.Business.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Service
{
    public interface IBookService : IDisposable
    {
        IEnumerable<Book> GetAllBooks();

        IEnumerable<Book> GetAllBooks(Expression<Func<Book, bool>> filter);

        IEnumerable<Book> GetAllBooks(Expression<Func<Book, bool>> filter, string[] includedProperties);

        IEnumerable<Book> GetAllBooks(Expression<Func<Book, bool>> filter, string[] includedProperties,
            Func<IQueryable<Book>, IOrderedQueryable<Book>> orderBy, out int totalCount, int? page = null, int? pageSize = null);

        Book FindBook(int bookId);

        void AddBook(Book book);

        void UpdateBook(Book book);

        void DeleteBook(int bookId);

        DateTime GetPropablyBookAvailableDate(int bookId);

        bool BookIsAvailable(int bookId);

        bool BookExistsInRents(int bookId);

        bool BookExistsInReservations(int bookId);
    }
}
