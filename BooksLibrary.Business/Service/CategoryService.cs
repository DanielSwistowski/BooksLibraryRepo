using BooksLibrary.Business.Models;
using BooksLibrary.Business.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork unitOfWork;
        public CategoryService(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return unitOfWork.CategoryRepository.Get();
        }

        public void AddCategory(Category category)
        {
            unitOfWork.CategoryRepository.Add(category);
            unitOfWork.Commit();
        }

        public void UpdateCategory(Category category)
        {
            unitOfWork.CategoryRepository.Update(category);
            unitOfWork.Commit();
        }

        public void DeleteCategory(int categoryId)
        {
            Category category = unitOfWork.CategoryRepository.GetById(categoryId);
            unitOfWork.CategoryRepository.Delete(category);
            unitOfWork.Commit();
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }
    }
}
