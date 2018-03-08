using BooksLibrary.Business.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BooksLibrary.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;
        public CategoryController(ICategoryService cs)
        {
            categoryService = cs;
        }

        public PartialViewResult CategoriesPartial(string selectedCategory, bool management)
        {
            ViewBag.CategoryFilter = selectedCategory;

            string actionName;
            if (management == true)
                actionName = "IndexBookManagement";
            else
                actionName = "Index";
            ViewBag.Action = actionName;

            var categories = categoryService.GetAllCategories().ToList();
            return PartialView("_CategoriesPartial", categories);
        }

        protected override void Dispose(bool disposing)
        {
            categoryService.Dispose();
            base.Dispose(disposing);
        }
    }
}
