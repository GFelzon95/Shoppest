using Microsoft.AspNetCore.Mvc;
using Shoppest.DataAccess;
using Shoppest.Models;
using Shoppest.Models.ViewModels;

namespace Shoppest.Controllers
{
    public class ProductCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var viewModel = new ProductCategoryIndex() { ProductCategories = _context.ProductCategories.ToList() };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            var viewModel = new ProductCategoryForm()
            {
                Id = 0,
                FormSettings = new FormSettings(FormSettings.Option.Create),
                IsDelete = false
            };

            return View("CategoryForm", viewModel);
        }

        [HttpPost]
        public IActionResult Create(ProductCategoryForm form)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new ProductCategoryForm()
                {
                    Id = form.Id,
                    Name = form.Name,
                    FormSettings = new FormSettings(FormSettings.Option.Create),
                    IsDelete = false
                };

                return View("CategoryForm", viewModel);
            }

            var category = new ProductCategory()
            {
                Name = form.Name,
            };
            _context.ProductCategories.Add(category);
            _context.SaveChanges();
            TempData["success"] = "Category successfully created.";
            return RedirectToAction("Index");

        }

        public IActionResult Edit(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var categoryInDb = _context.ProductCategories.SingleOrDefault(c => c.Id == id);

            if (categoryInDb == null)
            {
                return NotFound();
            }

            var viewModel = new ProductCategoryForm()
            {
                Id = categoryInDb.Id,
                Name = categoryInDb.Name,
                FormSettings = new FormSettings(FormSettings.Option.Edit),
                IsDelete = false
            };
            return View("CategoryForm", viewModel);
        }

        [HttpPost]
        public IActionResult Edit(ProductCategoryForm form)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new ProductCategoryForm()
                {
                    Id = form.Id,
                    Name = form.Name,
                    FormSettings = new FormSettings(FormSettings.Option.Edit),
                    IsDelete = false
                };

                return View("CategoryForm", viewModel);
            }

            var category = new ProductCategory()
            {
                Id = form.Id,
                Name = form.Name
            };

            _context.ProductCategories.Update(category);
            _context.SaveChanges();
            TempData["success"] = "Category successfully updated.";
            return RedirectToAction("Index");

        }
        public IActionResult Delete(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var categoryInDb = _context.ProductCategories.SingleOrDefault(c => c.Id == id);

            if (categoryInDb == null)
            {
                return NotFound();
            }

            var viewModel = new ProductCategoryForm()
            {
                Id = categoryInDb.Id,
                Name = categoryInDb.Name,
                FormSettings = new FormSettings(FormSettings.Option.Delete),
                IsDelete = true
            };
            return View("CategoryForm", viewModel);
        }

        [HttpPost]
        public IActionResult Delete(ProductCategoryForm form)
        {
            var category = _context.ProductCategories.SingleOrDefault(c => c.Id == form.Id);

            if (category == null)
            {
                return NotFound();
            }

            _context.ProductCategories.Remove(category);
            _context.SaveChanges();
            TempData["success"] = "Category successfully deleted.";
            return RedirectToAction("Index");
        }
    }
}
