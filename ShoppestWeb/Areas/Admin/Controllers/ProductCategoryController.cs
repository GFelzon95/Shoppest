using Microsoft.AspNetCore.Mvc;
using Shoppest.DataAccess.Repository.IRepository;
using Shoppest.Models;
using Shoppest.Models.ViewModels;

namespace ShoppestWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var viewModel = new ProductCategoryIndexVM() { ProductCategories = _unitOfWork.ProductCategories.GetAll() };
            return View(viewModel);
        }

        public IActionResult Upsert(int? id)
        {
            ProductCategoryFormVM viewModel;

            if (id == null || id == 0)
            {
                viewModel = new ProductCategoryFormVM()
                {
                    FormSettings = new FormSettings(FormSettings.Option.Create),
                    IsDelete = false
                };
            }
            else
            {
                var categoryInDb = _unitOfWork.ProductCategories.Get(c => c.Id == id);

                if (categoryInDb == null)
                {
                    return NotFound();
                }

                viewModel = new ProductCategoryFormVM()
                {
                    Id = categoryInDb.Id,
                    Name = categoryInDb.Name,
                    FormSettings = new FormSettings(FormSettings.Option.Edit),
                    IsDelete = false
                };
            }
            return View("CategoryForm", viewModel);

        }

        [HttpPost]
        public IActionResult Upsert(ProductCategoryFormVM form)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new ProductCategoryFormVM()
                {
                    Id = form.Id,
                    Name = form.Name,
                    FormSettings = new FormSettings(FormSettings.Option.Create),
                    IsDelete = false
                };

                return View("CategoryForm", viewModel);
            }

            ProductCategory category;

            if (form.Id == 0 || form.Id == null)
            {
                category = new ProductCategory()
                {
                    Name = form.Name,
                };
                _unitOfWork.ProductCategories.Add(category);
            }
            else
            {
                category = new ProductCategory()
                {
                    Id = form.Id,
                    Name = form.Name
                };

                _unitOfWork.ProductCategories.Update(category);
            }
            _unitOfWork.Save();
            TempData["success"] = "Category successfully " + (form.Id == 0 ? "created." : "updated.");
            return RedirectToAction("Index");

        }

        public IActionResult Delete(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var categoryInDb = _unitOfWork.ProductCategories.Get(c => c.Id == id);

            if (categoryInDb == null)
            {
                return NotFound();
            }

            var viewModel = new ProductCategoryFormVM()
            {
                Id = categoryInDb.Id,
                Name = categoryInDb.Name,
                FormSettings = new FormSettings(FormSettings.Option.Delete),
                IsDelete = true
            };
            return View("CategoryForm", viewModel);
        }

        [HttpPost]
        public IActionResult Delete(ProductCategoryFormVM form)
        {
            var category = _unitOfWork.ProductCategories.Get(c => c.Id == form.Id);

            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.ProductCategories.Remove(category);
            _unitOfWork.Save();
            TempData["success"] = "Category successfully deleted.";
            return RedirectToAction("Index");
        }
    }
}
