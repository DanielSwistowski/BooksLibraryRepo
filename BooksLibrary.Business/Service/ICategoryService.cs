using BooksLibrary.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Service
{
    public interface ICategoryService : IDisposable
    {
        IEnumerable<Category> GetAllCategories();

        void AddCategory(Category category);

        void UpdateCategory(Category category);

        void DeleteCategory(int categoryId);
    }
}
