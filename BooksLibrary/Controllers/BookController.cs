using AutoMapper;
using BooksLibrary.Business.Models;
using BooksLibrary.Business.Service;
using BooksLibrary.Models;
using BooksLibrary.Models.BookViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace BooksLibrary.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookService bookService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;
        public BookController(IBookService bs, ICategoryService cs, IMapper map)
        {
            bookService = bs;
            categoryService = cs;
            mapper = map;
        }

        [AllowAnonymous]
        public ActionResult Index(int? page, string searchBook, string searchAuthor, string sortOrder, string categoryFilter)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            int totalCount;
            string[] includeProperties = { "BookCategory" };

            ViewBag.SearchBook = searchBook;
            ViewBag.SearchAuthor = searchAuthor;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CategoryFilter = categoryFilter;

            IEnumerable<Book> query = bookService.GetAllBooks(CreateSearchFilter(searchBook, searchAuthor, categoryFilter), includeProperties, SelectSortOrder(sortOrder), out totalCount, pageNumber, pageSize);
            IEnumerable<IndexAllBooksViewModel> booksViewModel = mapper.Map<IEnumerable<IndexAllBooksViewModel>>(query);
            var books = new StaticPagedList<IndexAllBooksViewModel>(booksViewModel, pageNumber, pageSize, totalCount);

            return View(books);
        }

        private Expression<Func<Book, bool>> CreateSearchFilter(string searchBook, string searchAuthor, string categoryFilter)
        {
            Expression<Func<Book, bool>> filter = null;

            if (!string.IsNullOrEmpty(searchBook) && !string.IsNullOrEmpty(searchAuthor) && !string.IsNullOrEmpty(categoryFilter))
            {
                filter = f => f.Title.ToUpper().Contains(searchBook.ToUpper()) && f.Author.ToUpper().Contains(searchAuthor.ToUpper()) && f.BookCategory.CategoryName == categoryFilter;
            }
            else if (!string.IsNullOrEmpty(searchBook) && !string.IsNullOrEmpty(searchAuthor) && string.IsNullOrEmpty(categoryFilter))
            {
                filter = f => f.Title.ToUpper().Contains(searchBook.ToUpper()) && f.Author.ToUpper().Contains(searchAuthor.ToUpper());
            }
            else if (!string.IsNullOrEmpty(searchBook) && string.IsNullOrEmpty(searchAuthor) && !string.IsNullOrEmpty(categoryFilter))
            {
                filter = f => f.Title.ToUpper().Contains(searchBook.ToUpper()) && f.BookCategory.CategoryName == categoryFilter;
            }
            else if (string.IsNullOrEmpty(searchBook) && !string.IsNullOrEmpty(searchAuthor) && !string.IsNullOrEmpty(categoryFilter))
            {
                filter = f => f.Author.ToUpper().Contains(searchAuthor.ToUpper()) && f.BookCategory.CategoryName == categoryFilter;
            }
            else if (!string.IsNullOrEmpty(searchAuthor))
            {
                filter = f => f.Author.ToUpper().Contains(searchAuthor.ToUpper());
            }
            else if (!string.IsNullOrEmpty(searchBook))
            {
                filter = f => f.Title.ToUpper().Contains(searchBook.ToUpper());
            }
            else if (!string.IsNullOrEmpty(categoryFilter))
            {
                filter = f => f.BookCategory.CategoryName == categoryFilter;
            }

            return filter;
        }

        private Func<IQueryable<Book>, IOrderedQueryable<Book>> SelectSortOrder(string sortOrder)
        {
            Func<IQueryable<Book>, IOrderedQueryable<Book>> orderBy = null;
            switch (sortOrder)
            {
                case "title":
                    orderBy = q => q.OrderBy(b => b.Title);
                    break;
                case "author":
                    orderBy = q => q.OrderBy(b => b.Author);
                    break;
                default:
                    orderBy = q => q.OrderBy(b => b.BookId);
                    break;
            }

            return orderBy;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexBookManagement(int? page, string searchBook, string searchAuthor, string categoryFilter)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            int totalCount;
            ViewBag.SearchBook = searchBook;
            ViewBag.SearchAuthor = searchAuthor;

            string category;
            if (string.IsNullOrEmpty(categoryFilter))
            {
                category = categoryService.GetAllCategories().FirstOrDefault().CategoryName;
            }
            else
            {
                category = categoryFilter;
            }
            ViewBag.CategoryFilter = category;

            IEnumerable<Book> query = bookService.GetAllBooks(CreateSearchFilter(searchBook, searchAuthor, category), null, q => q.OrderBy(b => b.Title), out totalCount, pageNumber, pageSize);

            IEnumerable<BookViewModel> booksModel = mapper.Map<IEnumerable<BookViewModel>>(query);

            var allBooks = new StaticPagedList<BookViewModel>(booksModel, pageNumber, pageSize, totalCount);

            return View(allBooks);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddBook()
        {
            PopulateViewBagWithYears();
            PopulateViewBagWithCategories();
            return View();
        }

        private void PopulateViewBagWithYears()
        {
            var years = Enumerable.Range(1975, (DateTime.Now.Year - 1975) + 1).Select(year => new SelectListItem
                {
                    Value = year.ToString(CultureInfo.InvariantCulture),
                    Text = year.ToString(CultureInfo.InvariantCulture)
                });
            ViewBag.Years = years;
        }

        private void PopulateViewBagWithCategories()
        {
            var categories = categoryService.GetAllCategories().ToList();
            ViewBag.Categories = categories;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddBook(BookViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Book book = mapper.Map<Book>(viewModel);
                bookService.AddBook(book);
                TempData["message"] = "Książka została dodana!";
                return RedirectToAction("IndexBookManagement");
            }
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditBook(int bookId)
        {
            Book book = bookService.FindBook(bookId);

            if (book == null)
                return HttpNotFound();

            BookViewModel bookModel = mapper.Map<BookViewModel>(book);
            PopulateViewBagWithYears();
            PopulateViewBagWithCategories();
            return View(bookModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditBook(BookViewModel bookModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Book book = mapper.Map<Book>(bookModel);
                bookService.UpdateBook(book);
                TempData["message"] = "Zmiany zostały zapisane!";
                return RedirectToReferrerUrl(returnUrl);
            }
            return View(bookModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteBook(int bookId)
        {
            Book book = bookService.FindBook(bookId);
            if (book == null)
                return HttpNotFound();

            ViewBag.ReturnUrl = Request.UrlReferrer;

            BookViewModel bookModel = mapper.Map<BookViewModel>(book);

            return View(bookModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ActionName("DeleteBook")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBookConfirm(int bookId, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (bookService.BookExistsInRents(bookId))
            {
                Book book = bookService.FindBook(bookId);
                BookViewModel bookModel = mapper.Map<BookViewModel>(book);
                ModelState.AddModelError("", "Nie można usunąć wybranej książki, ponieważ jest ona wypożyczona!");
                return View(bookModel);
            }

            if (bookService.BookExistsInReservations(bookId))
            {
                Book book = bookService.FindBook(bookId);
                BookViewModel bookModel = mapper.Map<BookViewModel>(book);
                ModelState.AddModelError("", "Nie można usunąć wybranej książki, ponieważ jest ona zarezerwowana!");
                return View(bookModel);
            }

            bookService.DeleteBook(bookId);
            TempData["message"] = "Książka została usunięta!";
            return RedirectToReferrerUrl(returnUrl);
        }

        [AllowAnonymous]
        public ActionResult Details(int bookId)
        {
            Book book = bookService.FindBook(bookId);

            if (book == null)
                return HttpNotFound();

            BookViewModel bookModel = mapper.Map<BookViewModel>(book);
            return View(bookModel);
        }

        public ActionResult BookIsNotAvailable(int bookId)
        {
            Book book = bookService.FindBook(bookId);

            if (book == null)
                return HttpNotFound();

            var date = bookService.GetPropablyBookAvailableDate(bookId);

            BookIsNotAvailableViewModel model = new BookIsNotAvailableViewModel();

            model.Book = mapper.Map<BookViewModel>(book);

            model.PropablyBookAvailableDate = date;

            if (date == default(DateTime))
            {
                ViewBag.DateInfo = "Termin nieznany";
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Back(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult RedirectToReferrerUrl(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("IndexBookManagement", "Book");
        }

        protected override void Dispose(bool disposing)
        {
            bookService.Dispose();
            base.Dispose(disposing);
        }
    }
}
